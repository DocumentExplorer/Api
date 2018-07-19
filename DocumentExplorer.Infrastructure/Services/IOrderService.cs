using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentExplorer.Infrastructure.Services
{
    public interface IOrderService
    {
        Task AddOrderAsync(int id, string clientCountry, string clientIdentificationNumber,
            string brokerCountry, string brokerIdentificationNumber, string owner1Name);
    }
}
