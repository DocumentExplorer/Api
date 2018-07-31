using System;

namespace DocumentExplorer.Infrastructure.Commands.Orders
{
    public class DeleteOrder : AuthenticatedCommandBase, ICommand
    {
        public Guid OrderId { get; set;}
    }
}