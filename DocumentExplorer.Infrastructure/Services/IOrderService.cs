using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Infrastructure.DTO;

namespace DocumentExplorer.Infrastructure.Services
{
    public interface IOrderService
    {
        Task AddOrderAsync(Guid cacheId, int number, string clientCountry, 
            string clientIdentificationNumber, string brokerCountry, 
            string brokerIdentificationNumber, string owner1Name, string role);
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<IEnumerable<OrderDto>> GetAllByUserAsync(string username);
        Task<ExtendedOrderDto> GetAsync(Guid id);
        Task DeleteAsync(Guid id, string username);
        Task EditOrderAsync(Guid id, int number, string clientCountry, 
            string clientIdentificationNumber, string brokerCountry, 
            string brokerIdentificationNumber, string username);
        Task SetRequirementsAsync(Guid id, string fileType, 
            bool isRequired, string username);
        Task<LackingOrdersDto> GetLackingFilesAsync(string username, string role);
        Task ValidatePermissionsToOrder(string username, string role, Guid orderId);
        void ValidateIsNotComplementer(string role);
    }
}
