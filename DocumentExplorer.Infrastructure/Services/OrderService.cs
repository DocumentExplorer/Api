using AutoMapper;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.DTO;
using DocumentExplorer.Infrastructure.Exceptions;
using DocumentExplorer.Infrastructure.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DocumentExplorer.Infrastructure.Services
{
    public class OrderService : IOrderService, IService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IRealFileRepository _fileRepository;
        private readonly IFileRepository _fileDbRepository;
        private readonly ILogService _logService;
        private readonly IPermissionsService _permissionService;

        public OrderService(IOrderRepository orderRepository,
            IMapper mapper, IMemoryCache cache, IRealFileRepository fileRepository, IFileRepository fileDbRepository,
            ILogService logService, IPermissionsService permissionService)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _cache = cache;
            _fileRepository = fileRepository;
            _fileDbRepository = fileDbRepository;
            _logService = logService;
            _permissionService = permissionService;
        }


        public async Task AddOrderAsync(Guid cacheId, int number, string clientCountry, string clientIdentificationNumber, 
            string brokerCountry, string brokerIdentificationNumber, string owner1Name, string role)
        {
            if(!(role == Roles.User || role == Roles.Admin)) throw new UnauthorizedAccessException();
            var order = new Order(number, clientCountry, clientIdentificationNumber, brokerCountry,
                brokerIdentificationNumber, owner1Name, new DateTime());
            await _orderRepository.AddAsync(order);
            await _logService.AddLogAsync($"Dodano nowe zlecenie.",order,owner1Name);
            _cache.Set(cacheId,order.Id,TimeSpan.FromSeconds(5));
        }

        public async Task DeleteAsync(Guid id, string role, string username)
        {
            if(!(role == Roles.User || role == Roles.Admin)) throw new UnauthorizedAccessException();
            var order = await _orderRepository.GetOrFailAsync(id);
            var files = await _fileDbRepository.GetFilesContainingPath(order.GetPathToFolder());
            foreach(var file in files)
            {
                await _fileRepository.RemoveAsync(file.Path);
            }
            await _fileRepository.RemoveDirectoryIfExists(order.GetPathToFolder());
            await _orderRepository.RemoveAsync(order);
            await _logService.AddLogAsync($"Usunięto zlecenie.",order,username);
        }

        public async Task EditOrderAsync(Guid id, int number, string clientCountry, string clientIdentificationNumber, 
            string brokerCountry, string brokerIdentificationNumber, string username)
        {
            var order = await _orderRepository.GetOrFailAsync(id);
            var oldFolderName = order.GetPathToFolder();
            List<Core.Domain.File> files = await _fileDbRepository.GetFilesContainingPath(oldFolderName);
            order.SetNumber(number);
            order.SetClientCountry(clientCountry);
            order.SetClientIdentificationNumber(clientIdentificationNumber);
            order.SetBrokerCountry(brokerCountry);
            order.SetBrokerIdentificationNumber(brokerIdentificationNumber);
            var filePaths = files.Select(x => x.Path).ToList();
            List<string> newFilesPath = new List<string>();
            foreach(var path in filePaths)
            {
                var fileName = Path.GetFileNameWithoutExtension(path);
                Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
                Match result = re.Match(fileName);

                string alphaPart = result.Groups[1].Value;
                string numberPart = result.Groups[2].Value;
                if(!(alphaPart=="fvk"))
                {
                    numberPart = OrderFolderNameGenerator.AddLeadingZeros(number);
                }
                newFilesPath.Add($"{order.GetPathToFolder()}{alphaPart}{numberPart}.pdf");
                
            }
            var newFolderName = order.GetPathToFolder();
            await _fileRepository.UpdateFileNames(filePaths, newFilesPath);
            await _orderRepository.UpdateAsync(order);
            await _logService.AddLogAsync("Edytowano dane zlecenia.", order, username);
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            orders = orders.OrderBy(o => o.CreationDate);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetAllByUserAsync(string username)
        {
            var orders = await _orderRepository.GetAllByUser(username);
            orders = orders.OrderBy(o => o.CreationDate);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<ExtendedOrderDto> GetAsync(Guid id)
        {
            var order = await _orderRepository.GetOrFailAsync(id);
            return _mapper.Map<ExtendedOrderDto>(order);
        }

        public async Task<LackingOrdersDto> GetLackingFilesAsync(string username, string role)
        {
            var permissions = await _permissionService.GetPermissionsObjectAsync();
            var orders = await _orderRepository.GetAllAsync();
            if(role == Roles.User) orders = orders.Where(x=>x.Owner1Name==username);
            var date = DateTime.UtcNow.AddDays(-14);
            orders = orders.Where(x=>x.CreationDate<date);
            int lakingFilesInAllOrders = 0;
            var list = new List<LackingFilesDto>();
            foreach(var order in orders)
            {
                int lackingFiles = order.HasLackingFilesForRole(role,permissions);
                if(lackingFiles>0)
                {
                    var lackingFilesDto = new LackingFilesDto
                    {
                        Count = lackingFiles,
                        OrderId = order.Id
                    };
                    list.Add(lackingFilesDto);
                }
                lakingFilesInAllOrders += lackingFiles;
            }

            return new LackingOrdersDto
            {
                Count = lakingFilesInAllOrders,
                Orders = list
            };

        }

        public async Task SetRequirementsAsync(Guid id, string fileType, bool isRequired, string username, string role)
        {
            await _permissionService.Validate(fileType,role);
            var order = await _orderRepository.GetOrFailAsync(id);
            order.SetRequirements(fileType,isRequired);
            await _orderRepository.UpdateAsync(order);
            await _logService.AddLogAsync($"Zmieniono wymagania dla plik {fileType} na {isRequired}.", order, username);
        }
    }
}
