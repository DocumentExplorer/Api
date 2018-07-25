using System;

namespace DocumentExplorer.Core.Domain
{
    public class File
    {
        public File(Guid id, string path, Guid orderId)
        {
            Id = id;
            Path = path;
            OrderId = orderId;
        }
        public Guid Id { get; private set; }
        public string Path { get; set; }
        public Guid OrderId { get; private set; }

        public void SetOrderId(Guid orderId)
        {
            if(OrderId != Guid.Empty) throw new DomainException(ErrorCodes.FileIsAlreadyAssigned);
            OrderId = orderId;
        }
    }
}