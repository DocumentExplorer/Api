using System;

namespace DocumentExplorer.Infrastructure.DTO
{
    public class FileDto
    {
        public string FileType {get; set;}
        public bool IsRequired { get; set; }
        public string Path {get; set;}
    }
}