using System.Collections.Generic;

namespace DocumentExplorer.Infrastructure.DTO
{
    public class LackingOrdersDto
    {
        public int Count {get; set;}
        public IEnumerable<LackingFilesDto> Orders { get; set; }
    }
}