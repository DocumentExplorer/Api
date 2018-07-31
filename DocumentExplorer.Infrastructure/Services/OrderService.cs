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
        private readonly IHandler _handler;

        public OrderService(IOrderRepository orderRepository,
            IMapper mapper, IMemoryCache cache, IRealFileRepository fileRepository, IFileRepository fileDbRepository,
            ILogService logService, IPermissionsService permissionService, IHandler handler)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _cache = cache;
            _fileRepository = fileRepository;
            _fileDbRepository = fileDbRepository;
            _logService = logService;
            _permissionService = permissionService;
            _handler = handler;
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

        public async Task DeleteAsync(Guid id, string username)
        {
            var order = await _orderRepository.GetOrFailAsync(id);
            var files = await _fileDbRepository.GetFilesContainingPath(order.GetPathToFolder());
            foreach(var file in files)
            {
                await _fileRepository.RemoveAsync(file.Path);
                await _fileDbRepository.RemoveAsync(file);
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
            var oldOrder = OrderFolderNameGenerator.NameToOrder(oldFolderName);
            List<Core.Domain.File> files = await _fileDbRepository.GetFilesContainingPath(oldFolderName);
            await _handler
                .Run(async () => 
                {
                    order.SetNumber(number);
                    order.SetClientCountry(clientCountry);
                    order.SetClientIdentificationNumber(clientIdentificationNumber);
                    order.SetBrokerCountry(brokerCountry);
                    order.SetBrokerIdentificationNumber(brokerIdentificationNumber);
                    await _orderRepository.UpdateAsync(order);
                })
                .PropagateException()
                .Next()
                .Run(async () =>
                {
                    await UpdateFileRoots(files, number, order);
                })
                .OnSuccess(async () =>
                {
                    await _logService.AddLogAsync("Edytowano dane zlecenia.", order, username);
                })
                .OnError(async x => {
                    order.SetNumber(oldOrder.Number);
                    order.SetClientCountry(oldOrder.ClientCountry);
                    order.SetClientIdentificationNumber(oldOrder.ClientIdentificationNumber);
                    order.SetBrokerCountry(oldOrder.BrokerCountry);
                    order.SetBrokerIdentificationNumber(oldOrder.BrokerIdentificationNumber);
                    await _orderRepository.UpdateAsync(order);
                }, true)
                .Next()
                .ExecuteAllAsync();
        }

        private async Task UpdateFileRoots(List<Core.Domain.File> files, int number, Order order)
        {
            var filePaths = files.Select(x => x.Path).ToList();
            List<string> newFilesPath = new List<string>();
            foreach(var file in files)
            {
                var path = file.Path;
                var fileName = Path.GetFileNameWithoutExtension(path);
                Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
                Match result = re.Match(fileName);

                string alphaPart = result.Groups[1].Value;
                string numberPart = result.Groups[2].Value;
                if(!(alphaPart=="fvk"))
                {
                    numberPart = OrderFolderNameGenerator.AddLeadingZeros(number);
                }
                file.Path = $"{order.GetPathToFolder()}{alphaPart}{numberPart}.pdf";
                newFilesPath.Add(file.Path);
                await _fileDbRepository.UpdateAsync(file);
                
            }
            var newFolderName = order.GetPathToFolder();
            await _fileRepository.UpdateFileNames(filePaths, newFilesPath);
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

        public async Task SetRequirementsAsync(Guid id, string fileType, bool isRequired, string username)
        {
            var order = await _orderRepository.GetOrFailAsync(id);
            await _handler
                .Run(async ()=>
                {
                    order.SetRequirements(fileType,isRequired);
                    await _orderRepository.UpdateAsync(order);
                })
                .OnSuccess(async ()=>
                {
                    await _logService
                    .AddLogAsync($"Zmieniono wymagania dla plik {fileType} na {isRequired}.", order, username);
                })
                .PropagateException()
                .ExecuteAsync();
            
            
        }

        public void ValidateIsNotComplementer(string role)
        {
            if(role == Roles.Complementer) throw new UnauthorizedAccessException();
        }

        public async Task ValidatePermissionsToOrder(string username, string role, Guid orderId)
        {
            if(role==Roles.Admin || role == Roles.Complementer) return;
            var order = await _orderRepository.GetAsync(orderId);
            if(order.Owner1Name==username) return;
            throw new UnauthorizedAccessException(); 
        }
    }
}
