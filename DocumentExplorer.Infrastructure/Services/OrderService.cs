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
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IRealFileRepository _fileRepository;
        private readonly IFileRepository _fileDbRepository;

        public OrderService(IOrderRepository orderRepository,
            IMapper mapper, IMemoryCache cache, IRealFileRepository fileRepository, IFileRepository fileDbRepository)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _cache = cache;
            _fileRepository = fileRepository;
            _fileDbRepository = fileDbRepository;
        }


        public async Task AddOrderAsync(Guid cacheId, int number, string clientCountry, string clientIdentificationNumber, 
            string brokerCountry, string brokerIdentificationNumber, string owner1Name, string role)
        {
            if(!(role == Roles.User || role == Roles.Admin)) throw new UnauthorizedAccessException();
            var order = new Order(number, clientCountry, clientIdentificationNumber, brokerCountry,
                brokerIdentificationNumber, owner1Name, new DateTime());
            await _orderRepository.AddAsync(order);
            _cache.Set(cacheId,order.Id,TimeSpan.FromSeconds(5));
        }

        public async Task DeleteAsync(Guid id)
        {
            var order = await _orderRepository.GetOrFailAsync(id);
            await _orderRepository.RemoveAsync(order);
        }

        public async Task EditOrderAsync(Guid id, int number, string clientCountry, string clientIdentificationNumber, 
            string brokerCountry, string brokerIdentificationNumber)
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

        public async Task SetRequirementsAsync(Guid id, string fileType, bool isRequired)
        {
            var order = await _orderRepository.GetOrFailAsync(id);
            order.SetRequirements(fileType,isRequired);
            await _orderRepository.UpdateAsync(order);
        }
    }
}
