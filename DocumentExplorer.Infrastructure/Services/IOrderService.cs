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
        Task AddOrderAsync(int id, string clientCountry, string clientIdentificationNumber,
            string brokerCountry, string brokerIdentificationNumber, string owner1Name);
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<OrderDto> GetAsync(int id);
        Task DeleteAsync(int id);
    }
}
