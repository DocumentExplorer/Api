using System;

namespace DocumentExplorer.Core.Domain
{
    public class File
    {
        public Guid Id { get; private set; }
        public string Path { get; private set; }
        public Guid OrderId { get; private set; }
        public string FileType { get; private set; }
        public bool IsRequired { get; private set; }

        public File(Guid orderId, string fileType)
        {
            Id = Guid.NewGuid();
            Path = string.Empty;
            SetOrderId(orderId);
            SetFileType(fileType);
            IsRequired = true;
        }

        public void SetFile(string path)
        {
            if(!IsRequired) throw new DomainException(ErrorCodes.FileIsNotRequired);
            if(Path!=string.Empty) throw new DomainException(ErrorCodes.FileIsAlreadyAssigned);
            Path = path;
        }

        public void SetIsNotRequired()
        {
            if(Path!=string.Empty) throw new DomainException(ErrorCodes.FileIsAlreadyAssigned);
            IsRequired = false;
        }

        public void SetIsRequired()
        {
            IsRequired = true;
        }

        public void DeleteFile()
        {
            Path = string.Empty;
        }

        protected File()
        {

        }
        private void SetOrderId(Guid orderId)
        {
            if(orderId == Guid.Empty) throw new DomainException(ErrorCodes.InvalidOrderId);
            OrderId = orderId;
        }

        private void SetFileType(string fileType)
        {
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