using AutoMapper;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.DTO;
using DocumentExplorer.Infrastructure.Exceptions;
using DocumentExplorer.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentExplorer.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderFolderNameGenerator _orderFolderNameGenerator;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, 
            IOrderFolderNameGenerator orderFolderNameGenerator,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _orderFolderNameGenerator = orderFolderNameGenerator;
            _mapper = mapper;
        }


        public async Task AddOrderAsync(int id, string clientCountry, string clientIdentificationNumber, 
            string brokerCountry, string brokerIdentificationNumber, string owner1Name)
        {
            var order = await _orderRepository.GetAsync(id);
            if (order != null) throw new ServiceException(Exceptions.ErrorCodes.OrderIdInUse);
            order = new Order(id, clientCountry, clientIdentificationNumber, brokerCountry,
                brokerIdentificationNumber, owner1Name, new DateTime());
            order.PathToFolder = _orderFolderNameGenerator.OrderToName(order);
            await _orderRepository.AddAsync(order);
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _orderRepository.GetOrFailAsync(id);
            await _orderRepository.RemoveAsync(order);
        }

        public async Task EditOrderAsync(int id, string clientCountry, string clientIdentificationNumber, 
            string brokerCountry, string brokerIdentificationNumber, string owner2Name, string username)
        {
            var order = await _orderRepository.GetOrFailAsync(id);
            order.SetClientCountry(clientCountry);
            order.SetClientIdentificationNumber(clientIdentificationNumber);
            order.SetBrokerCountry(brokerCountry);
            order.SetBrokerIdentificationNumber(brokerIdentificationNumber);
            order.SetOwner2Name(owner2Name, username);
            await _orderRepository.UpdateAsync(order);
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            orders = orders.OrderBy(o => o.Id);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> GetAsync(int id)
        {
            var order = await _orderRepository.GetOrFailAsync(id);
            return _mapper.Map<OrderDto>(order);
        }
    }
}
