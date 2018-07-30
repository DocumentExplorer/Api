using System;

namespace DocumentExplorer.Infrastructure.Commands.Orders
{
    public class GetLackingFiles : AuthenticatedCommandBase, ICommand
    {
        public Guid LackingFilesId {get; set;}
    }
}