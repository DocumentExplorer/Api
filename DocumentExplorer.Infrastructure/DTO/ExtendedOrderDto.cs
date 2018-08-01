using System;
using System.Collections.Generic;

namespace DocumentExplorer.Infrastructure.DTO
{
    public class ExtendedOrderDto : OrderDto
    {
        public Guid CMRId {get; set;}
        public bool IsCMRRequired { get; set;}
        public Guid FVKId {get; set;}
        public bool IsFVKRequired { get; set;}
        public Guid FVPId {get; set;}
        public bool IsFVPRequired { get; set;}
        public Guid NIPId {get; set;}
        public bool IsNIPRequired { get; set;}
        public Guid NotaId {get; set;}
        public bool IsNotaRequired { get; set;}
        public Guid PPId {get; set;}
        public bool IsPPRequired { get; set;}
        public Guid RKId {get; set;}
        public bool IsRKRequired { get; set;}
        public Guid ZKId {get; set;}
        public bool IsZKRequired { get; set;}
        public Guid ZPId {get; set;}
        public bool IsZPRequired { get; set; }

        public IEnumerable<FileDto> Files { get; set; }
    }
}