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
            bool isCorrect = false;
            switch(fileType)
            {
                case "cmr":
                    isCorrect = true;
                    break;
                case "fvk":
                    isCorrect = true;
                    break;
                case "fvp":
                    isCorrect = true;
                    break;
                case "nip":
                    isCorrect = true;
                    break;
                case "nota":
                    isCorrect = true;
                    break;
                case "pp":
                    isCorrect = true;
                    break;
                case "rk":
                    isCorrect = true;
                    break;
                case "zk":
                    isCorrect = true;
                    break;
                case "zp":
                    isCorrect = true;
                    break;
            }
            if(!isCorrect) throw new DomainException(ErrorCodes.InvalidFileType);
            FileType = fileType;
        }
    }
}