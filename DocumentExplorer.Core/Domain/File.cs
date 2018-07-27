using System;

namespace DocumentExplorer.Core.Domain
{
    public class File
    {
        public File(Guid id, string path, Guid orderId, string fileType=null)
        {
            Id = id;
            Path = path;
            OrderId = orderId;
            SetFileType(fileType);
        }

        protected File()
        {

        }
        public Guid Id { get; private set; }
        public string Path { get; set; }
        public Guid OrderId { get; private set; }
        public string FileType { get; private set; }
        public void SetOrderId(Guid orderId)
        {
            if(OrderId != Guid.Empty) throw new DomainException(ErrorCodes.FileIsAlreadyAssigned);
            OrderId = orderId;
        }

        public void SetFileType(string fileType)
        {
            if(fileType == null) return;
            bool isCorrect = false;
            var properties = typeof(FileTypes).GetProperties();
            foreach(var property in properties)
            {
                if(property.Name.ToLower()==fileType)
                {
                    isCorrect = true;
                }
            }
            if(!isCorrect) throw new DomainException(ErrorCodes.InvalidFileType);
            FileType = fileType;
        }
    }
}