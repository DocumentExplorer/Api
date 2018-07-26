using System;

namespace DocumentExplorer.Infrastructure.Commands.Orders
{
    public class SetRequirements : AuthenticatedCommandBase,ICommand
    {
        public Guid OrderId { get; set; }
        public string FileType { get; set; }
        public bool IsRequired { get; set; }
    }
}