using System;

namespace DocumentExplorer.Core.Domain
{
    public class File
    {
        public File(Guid id, string path)
        {
            Id = id;
            Path = path;
        }
        public Guid Id { get; private set; }
        public string Path { get; set; }
    }
}