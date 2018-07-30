using System;

namespace DocumentExplorer.Infrastructure.DTO
{
    public class LackingFilesDto
    {
        public int Count { get; set; }
        public Guid OrderId { get; set; }
    }
}