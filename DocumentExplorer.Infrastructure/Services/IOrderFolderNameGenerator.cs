using DocumentExplorer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentExplorer.Infrastructure.Services
{
    public interface IOrderFolderNameGenerator
    {
        string OrderToName(Order order);
        Order NameToOrder(string order);
        List<Order> ListOfOrders(List<string> orders)
    }
}
