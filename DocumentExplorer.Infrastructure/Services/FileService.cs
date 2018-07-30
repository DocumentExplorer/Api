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
    public class FileService : IFileService, IService
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
                await GoThroughMonthsAsync(Path.GetFileName(year));
            }
        }

        private async Task GoThroughMonthsAsync(string yearPath)
        {
            var months = await _realFileRepository.GetDirectoriesAsync(yearPath);
            foreach(var month in months)
            {
                await GoThroughOrdersAsync($"{yearPath}/{Path.GetFileName(month)}");
            }
        }

        private async Task GoThroughOrdersAsync(string path)
        {
            var orders = await _realFileRepository.GetDirectoriesAsync(path);
            foreach(var order in orders)
            {
                await GoThroughOrderAsync($"{path}/{Path.GetFileName(order)}");
            }
        }

        private async Task GoThroughOrderAsync(string path)
        {
            var order = OrderFolderNameGenerator.NameToOrder(path);
            order.CreationDate = await GenerateOrderCreationDateAsync(order, path);
            var secondOwner = path.Substring(path.Length - 4);
            order = await AddFilesAsync(order, path, secondOwner);
            order = SetNotAddedFilesAsNotRequired(order);
            await _orderRepository.AddAsync(order);
            await _logService.AddLogAsync($"Dodano nowe zlecenie.",order,order.Owner1Name,order.CreationDate);
            await AddOwnersAsync(order, secondOwner);
        }

        private async Task AddOwnersAsync(Order order, string secondOwner)
        {
            if((await _userRepository.GetAsync(order.Owner1Name))==null)
            {
                await _userService.RegisterAsync(order.Owner1Name,"secret123",Roles.User);
            }
            if((await _userRepository.GetAsync(secondOwner))==null)
            {
                try
                {
                    await _userService.RegisterAsync(secondOwner,"secret123",Roles.Complementer);
                }
                catch(Exception)
                {
                    
                }
            }
        }

        private Order SetNotAddedFilesAsNotRequired(Order order)
        {  
            var properties = typeof(FileTypes).GetProperties();
            foreach(var property in properties)
            {
                if(!order.FileIsAlreadyAssigned(property.Name))
                {
                    order.SetRequirements(property.Name.ToLower(), false);
                }
            }

            return order;
        }

        private async Task<Order> AddFilesAsync(Order order, string path, string secondOwner)
        {
            var filesPaths = await _realFileRepository.GetFilesPathsAsync(path);
            foreach(var filePath in filesPaths)
            {
                order = await AddFileAsync(order, $"{path}/{Path.GetFileName(filePath)}", secondOwner);
            }
            return order;
        }

        private async Task<Order> AddFileAsync(Order order, string path, string secondOwner)
        {
            var file = GenerateFile(path, order);
            int invoiceNumber = TryGetInvoiceNumber(path);
            order.LinkFile(file, file.FileType, invoiceNumber);
            await LogAddingFileAsync(file, order, secondOwner, path);
            await _fileRepository.AddAsync(file);
            return order;
        }

        private async Task LogAddingFileAsync(Core.Domain.File file, Order order, string secondOwner, string path)
        {
            var fileCreatonDate = await _realFileRepository.GetFileCreationDateAsync(path);
            var fileAdder = await GetFileAdderAsync(file, order, secondOwner);
            await _logService.AddLogAsync($"Dodano plik: {Path.GetFileName(file.Path)}",
            order,fileAdder,fileCreatonDate);
            Logger.Log(NLog.LogLevel.Info, file.Path);
        }

        private async Task<string> GetFileAdderAsync(Core.Domain.File file, Order order, string secondOwner)
        {
            var permissions = await _permissionService.GetPermissionsAsync();
            var properties = typeof(FileTypes).GetProperties();
            foreach(var property in properties)
            {
                if(property.Name.ToLower()==file.FileType)
                {
                    if(GetFilePermission(permissions, property.Name)=="user")
                    {
                        return order.Owner1Name;
                    }
                    else
                    {
                        return secondOwner;
                    }
                }
            }
            throw new ServiceException(Exceptions.ErrorCodes.InvalidFileType);
        }

        private string GetFilePermission(PermissionsDto permissions, string propertyName)
        {
            var propertyValueObject = typeof(PermissionsDto).GetProperty(propertyName).GetValue(permissions, null);
            if(propertyValueObject is string filePermission)
            {
                return filePermission;
            }
            throw new InvalidCastException();
        }
        private Core.Domain.File GenerateFile(string path, Order order)
        {
            var fileName = GetDividedFileName(path);
            return new Core.Domain.File(Guid.NewGuid(), path, order.Id, fileName[0]);
        }

        private int TryGetInvoiceNumber(string path)
        {
            var fileName = GetDividedFileName(path);
            if(fileName[0]=="fvk")
            {
                return int.Parse(fileName[1].TrimStart(new Char[] { '0' }));
            }
            else return 0;
        }

        private string[] GetDividedFileName(string path)
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
            Match result = re.Match(fileName);
            string alphaPart = result.Groups[1].Value;
            string numberPart = result.Groups[2].Value;
            var tab = new string[2];
            tab[0] = alphaPart;
            tab[1] = numberPart;
            return tab;
        }

        private async Task<DateTime> GenerateOrderCreationDateAsync(Order order, string path)
        {
            var directoryCreationDate = await _realFileRepository.GetDirectoryCreationDateAsync(path);
            return new DateTime(order.CreationDate.Year, order.CreationDate.Month, directoryCreationDate.Day, 
                        directoryCreationDate.Hour, directoryCreationDate.Minute, directoryCreationDate.Second);
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