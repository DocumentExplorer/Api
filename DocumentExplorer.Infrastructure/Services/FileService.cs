using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.DTO;
using DocumentExplorer.Infrastructure.Exceptions;
using DocumentExplorer.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NLog;

namespace DocumentExplorer.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IRealFileRepository _realFileRepository;
        private readonly IPermissionsService _permissionService;
        private readonly IMapper _mapper;
        private readonly ILogService _logService;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public FileService(IFileRepository fileRepository, IOrderRepository orderRepository, 
            IRealFileRepository realFileRepository, IMapper mapper, IPermissionsService permissionService,
            ILogService logService, IUserRepository userRepository, IUserService userService)
        {
            _fileRepository = fileRepository;
            _orderRepository = orderRepository;
            _realFileRepository = realFileRepository;
            _mapper = mapper;
            _permissionService = permissionService;
            _logService = logService;
            _userRepository = userRepository;
            _userService = userService;
        }

        public async Task DeleteFileAsync(Guid id, string role, string username)
        {
            var file = await _fileRepository.GetOrFailAsync(id);
            await _permissionService.Validate(file.FileType, role);
            var order = await _orderRepository.GetOrFailAsync(file.OrderId);
            await _realFileRepository.RemoveAsync(file.Path);
            await _logService.AddLogAsync($"Usunięto plik: {Path.GetFileName(file.Path)}", order, username);
            await _fileRepository.RemoveAsync(file);
            order.UnlinkFile(file.FileType);
            await _orderRepository.UpdateAsync(order);
        }

        public async Task GenerateAsync()
        {
            var years = await _realFileRepository.GetDirectoriesAsync("");
            foreach(var year in years)
            {
                var months = await _realFileRepository.GetDirectoriesAsync(Path.GetFileName(year));
                foreach(var month in months)
                {
                    var orders = await _realFileRepository
                        .GetDirectoriesAsync($"{Path.GetFileName(year)}/{Path.GetFileName(month)}");
                    foreach(var order in orders)
                    {
                        var orderObject = OrderFolderNameGenerator.NameToOrder($"{Path.GetFileName(year)}/{Path.GetFileName(month)}/{Path.GetFileName(order)}");
                        var directoryCreationDate = await _realFileRepository
                            .GetDirectoryCreationDateAsync($"{Path.GetFileName(year)}/{Path.GetFileName(month)}/{Path.GetFileName(order)}");
                        orderObject.CreationDate = new DateTime(orderObject.CreationDate.Year,
                                                                orderObject.CreationDate.Month,
                                                                directoryCreationDate.Day,
                                                                directoryCreationDate.Hour,
                                                                directoryCreationDate.Minute,
                                                                directoryCreationDate.Second);
                        var filesPaths = await _realFileRepository
                            .GetFilesPathsAsync($"{Path.GetFileName(year)}/{Path.GetFileName(month)}/{Path.GetFileName(order)}");
                        foreach(var filePath in filesPaths)
                        {
                            var fileName = Path.GetFileNameWithoutExtension(filePath);
                            Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
                            Match result = re.Match(fileName);

                            int invoiceNumber = 0;
                            string alphaPart = result.Groups[1].Value;
                            string numberPart = result.Groups[2].Value;
                            if(alphaPart=="fvk")
                            {
                                invoiceNumber = int.Parse(numberPart.TrimStart(new Char[] { '0' }));
                            }
                            var file = new Core.Domain.File(Guid.NewGuid(), 
                                $"{Path.GetFileName(year)}/{Path.GetFileName(month)}/{Path.GetFileName(order)}/{Path.GetFileName(filePath)}",
                                orderObject.Id, alphaPart);
                            orderObject.LinkFile(file, file.FileType, invoiceNumber);
                            await _fileRepository.AddAsync(file);
                            Logger.Log(NLog.LogLevel.Info, file.Path);
                        }
                        if(orderObject.CMRId==Guid.Empty) orderObject.SetRequirements("cmr", false);
                        if(orderObject.FVKId==Guid.Empty) orderObject.SetRequirements("fvk", false);
                        if(orderObject.FVPId==Guid.Empty) orderObject.SetRequirements("fvp", false);
                        if(orderObject.NIPId==Guid.Empty) orderObject.SetRequirements("nip", false);
                        if(orderObject.NotaId==Guid.Empty) orderObject.SetRequirements("nota", false);
                        if(orderObject.PPId==Guid.Empty) orderObject.SetRequirements("pp", false);
                        if(orderObject.RKId==Guid.Empty) orderObject.SetRequirements("rk", false);
                        if(orderObject.ZKId==Guid.Empty) orderObject.SetRequirements("zk", false);
                        if(orderObject.ZPId==Guid.Empty) orderObject.SetRequirements("zp", false);
                        await _orderRepository.AddAsync(orderObject);
                        if((await _userRepository.GetAsync(orderObject.Owner1Name))==null)
                        {
                            await _userService.RegisterAsync(orderObject.Owner1Name,"secret123",Roles.User);
                        }
                    }
                }
            }
        }

        public async Task<IEnumerable<FileDto>> GetAllFilesAsync()
        {
            var files = await _fileRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<FileDto>>(files);
        }

        public async Task<FileDto> GetFileAsync(Guid id)
        {
            var file = await _fileRepository.GetOrFailAsync(id);
            return _mapper.Map<FileDto>(file);
        }

        public async Task<MemoryStream> GetFileStreamAsync(Guid id)
        {
            var file = await _fileRepository.GetOrFailAsync(id);
            return await _realFileRepository.GetOrFailAsync(file.Path);
        }

        public async Task PutIntoLocationAsync(Guid uploadId, Guid orderId, string fileType, int invoiceNumber, string role, string username)
        {
            await _permissionService.Validate(fileType, role);
            var file = await _fileRepository.GetOrFailAsync(uploadId);
            var order = await _orderRepository.GetOrFailAsync(orderId);
            file.SetOrderId(order.Id);
            file.SetFileType(fileType);
            order.LinkFile(file,fileType,invoiceNumber);
            var destination = order.GetPathToFile(fileType);
            await _realFileRepository.AddAsync(file.Path, destination);
            file.Path = destination;
            await _logService.AddLogAsync($"Dodano plik: {Path.GetFileName(file.Path)}", order, username);
            await _orderRepository.UpdateAsync(order);
            await _fileRepository.UpdateAsync(file);
        }

        public async Task UploadAsync(IFormFile file, Guid id)
        {
            var filePath = Path.GetTempFileName();

            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var fileData = new Core.Domain.File(id,filePath, Guid.Empty);
            await _fileRepository.AddAsync(fileData);
        }

        public void Validate(IFormFile file)
        {
            if(file == null)
            {
                throw new ServiceException(Exceptions.ErrorCodes.NoFile);
            }
            if(file.FileName == null)
            {
                throw new ServiceException(Exceptions.ErrorCodes.NoFileName);
            }
            if(!file.FileName.EndsWith(".pdf"))
            {
                throw new ServiceException(Exceptions.ErrorCodes.InvalidExtension);
            }
            if(file.Length <= 0)
            {
                throw new ServiceException(Exceptions.ErrorCodes.FileHasNoData);
            }
        }
    }
}