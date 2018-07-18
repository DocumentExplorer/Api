using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentExplorer.Infrastructure.BlobStorage
{
    public class BlobStorageSettings
    {
        public string StorageAccount { get; set; }
        public string StorageKey { get; set; }
        public string ContainerName { get; set; }
    }
}
