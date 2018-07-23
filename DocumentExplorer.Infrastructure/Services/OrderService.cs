using AutoMapper;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.DTO;
using DocumentExplorer.Infrastructure.Exceptions;
using DocumentExplorer.Infrastructure.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentExplorer.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly IRealFileRepository _fileRepository;

        public OrderService(IOrderRepository orderRepository,
            IMapper mapper, IMemoryCache cache, IRealFileRepository fileRepository)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _cache = cache;
            _fileRepository = fileRepository;
        }


        public async Task AddOrderAsync(Guid cacheId, int number, string clientCountry, string clientIdentificationNumber, 
            string brokerCountry, string brokerIdentificationNumber, string owner1Name)
        {
            var order = new Order(number, clientCountry, clientIdentificationNumber, brokerCountry,
                brokerIdentificationNumber, owner1Name, new DateTime());
            order.PathToFolder = OrderFolderNameGenerator.OrderToName(order);
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
            var oldFolderName = order.PathToFolder;
            order.SetNumber(number);
            order.SetClientCountry(clientCountry);
            order.SetClientIdentificationNumber(clientIdentificationNumber);
            order.SetBrokerCountry(brokerCountry);
            order.SetBrokerIdentificationNumber(brokerIdentificationNumber);
            var newFolderName = OrderFolderNameGenerator.OrderToName(order);
            await _fileRepository.UpdateFolderName(oldFolderName, newFolderName);
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

        public async Task<OrderDto> GetAsync(Guid id)
        {
            var order = await _orderRepository.GetOrFailAsync(id);
            return _mapper.Map<OrderDto>(order);
        }
    }
}
