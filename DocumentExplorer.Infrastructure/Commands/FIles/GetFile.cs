using System;

namespace DocumentExplorer.Infrastructure.Commands.Files
{
    public class GetFile : AuthenticatedCommandBase, ICommand
    {
        public Guid OrderId { get; set; }
        public string FileType { get; set; }
        public Guid CacheId { get; set; }
    }
}