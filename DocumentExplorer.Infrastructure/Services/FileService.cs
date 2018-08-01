using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly IOrderRepository _orderRepository;
        private readonly IRealFileRepository _realFileRepository;
        private readonly IPermissionsService _permissionService;
        private readonly IMapper _mapper;
        private readonly ILogService _logService;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IOrderService _orderService;
        private readonly IHandler _handler;
        private readonly IMemoryCache _cache;

        public FileService(IOrderRepository orderRepository, 
            IRealFileRepository realFileRepository, IMapper mapper, IPermissionsService permissionService,
            ILogService logService, IUserRepository userRepository, IUserService userService,
            IOrderService orderService, IHandler handler, IMemoryCache cache)
        {
            _orderRepository = orderRepository;
            _realFileRepository = realFileRepository;
            _mapper = mapper;
            _permissionService = permissionService;
            _logService = logService;
            _userRepository = userRepository;
            _userService = userService;
            _orderService = orderService;
            _handler = handler;
            _cache = cache;
        }

        public async Task DeleteFileAsync(Guid id, string fileType, string role, string username)
        {
            var order = await _orderRepository.GetOrFailAsync(id);
            await _handler
            .Validate(async () =>
            {
                await _orderService.ValidatePermissionsToOrder(username, role, order.Id);
                await _permissionService.Validate(fileType, role);
            })
            .Run(async ()=>
            {
                var file = order.Files.SingleOrDefault(x=> x.FileType==fileType);
                await _realFileRepository.RemoveAsync(file.Path);
                await _logService.AddLogAsync($"Usunięto plik: {Path.GetFileName(file.Path)}", order, username);
                order.UnlinkFile(file.FileType);
                await _orderRepository.UpdateAsync(order);
            })
            .OnCustomError(x => throw new ServiceException(x.Code), true)
            .ExecuteAsync();
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
                if(!order.FileIsAlreadyAssigned(property.Name.ToLower()))
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
            var fileType = GetFileType(path);
            int invoiceNumber = TryGetInvoiceNumber(path);
            order.LinkFile(fileType, invoiceNumber);
            await LogAddingFileAsync(fileType, order, secondOwner, path);
            return order;
        }

        private async Task LogAddingFileAsync(string fileType, Order order, string secondOwner, string path)
        {
            var fileCreatonDate = await _realFileRepository.GetFileCreationDateAsync(path);
            var fileAdder = await GetFileAdderAsync(path, order, secondOwner);
            await _logService.AddLogAsync($"Dodano plik: {Path.GetFileName(path)}",
            order,fileAdder,fileCreatonDate);
            Logger.Log(NLog.LogLevel.Info, path);
        }

        private async Task<string> GetFileAdderAsync(string fileType, Order order, string secondOwner)
        {
            var permissions = await _permissionService.GetPermissionsAsync();
            var properties = typeof(FileTypes).GetProperties();
            foreach(var property in properties)
            {
                if(property.Name.ToLower()==fileType)
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
        private string GetFileType(string path)
        {
            var fileType = GetDividedFileName(path);
            return fileType[0];
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

        public async Task<MemoryStream> GetFileStreamAsync(Guid orderId, string fileType)
        {
            var order = await _orderRepository.GetOrFailAsync(orderId);
            var filePath = order.GetPathToFile(fileType);
            return await _realFileRepository.GetOrFailAsync(filePath);
        }

        public async Task PutIntoLocationAsync(Guid uploadId, Guid orderId, string fileType, 
            int invoiceNumber, string role, string username)
        {
            var filePath = _cache.GetString(uploadId);
            var order = await _orderRepository.GetOrFailAsync(orderId);
            order.LinkFile(fileType, invoiceNumber);
            var destination = order.GetPathToFile(fileType);
            await _realFileRepository.AddAsync(filePath, destination);
            await _logService.AddLogAsync($"Dodano plik: {Path.GetFileName(destination)}", order, username);
            await _orderRepository.UpdateAsync(order);
        }

        public async Task UploadAsync(IFormFile file, Guid id)
        {
            var filePath = Path.GetTempFileName();

            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            _cache.Set(id, filePath, TimeSpan.FromMinutes(10));
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