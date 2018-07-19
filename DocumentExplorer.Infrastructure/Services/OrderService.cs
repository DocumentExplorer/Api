using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.Exceptions;
using System;
using System.Threading.Tasks;

namespace DocumentExplorer.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderFolderNameGenerator _orderFolderNameGenerator;

        public OrderService(IOrderRepository orderRepository, 
            IOrderFolderNameGenerator orderFolderNameGenerator)
        {
            _orderRepository = orderRepository;
            _orderFolderNameGenerator = orderFolderNameGenerator;
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
    }
}
