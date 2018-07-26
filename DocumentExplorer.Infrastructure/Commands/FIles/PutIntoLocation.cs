using System;

namespace DocumentExplorer.Infrastructure.Commands.Files
{
    public class PutIntoLocation : AuthenticatedCommandBase, ICommand
    {
        public Guid UploadId { get; set; }
        public Guid OrderId { get; set; }
        public string FileType { get; set; }
        public int InvoiceNumber { get; set; }
    }
}