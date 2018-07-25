using System;

namespace DocumentExplorer.Infrastructure.DTO
{
    public class FileDto
    {
        public Guid Id { get; set; }
        public string Path { get; set; }
        public Guid OrderId { get; set; }
        public string FileType {get; set;}
    }
}