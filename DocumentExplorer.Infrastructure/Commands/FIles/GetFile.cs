using System;

namespace DocumentExplorer.Infrastructure.Commands.Files
{
    public class GetFile : AuthenticatedCommandBase, ICommand
    {
        public Guid FileId { get; set; }
        public Guid CacheId { get; set; }
    }
}