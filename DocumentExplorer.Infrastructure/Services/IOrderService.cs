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
        Task AddOrderAsync(Guid cacheId, int number, string clientCountry, string clientIdentificationNumber,
            string brokerCountry, string brokerIdentificationNumber, string owner1Name);
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<IEnumerable<OrderDto>> GetAllByUserAsync(string username);
        Task<ExtendedOrderDto> GetAsync(Guid id);
        Task DeleteAsync(Guid id);
        Task EditOrderAsync(Guid id, int number, string clientCountry, string clientIdentificationNumber, 
            string brokerCountry, string brokerIdentificationNumber);
        Task SetRequirementsAsync(Guid id, string fileType, bool isRequired);
    }
}
