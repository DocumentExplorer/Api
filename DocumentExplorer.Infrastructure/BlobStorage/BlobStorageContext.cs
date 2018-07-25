using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentExplorer.Infrastructure.BlobStorage
{
    public class BlobStorageContext
    {
        private readonly BlobStorageSettings _blobStorageSettings;

        public BlobStorageContext(BlobStorageSettings blobStorageSettings)
        {
            _blobStorageSettings = blobStorageSettings;
        }

        public async Task<List<AzureBlobItem>> ListAsync()
        {
            return await GetBlobListAsync();
        }

        public async Task<List<string>> ListFoldersAsync()
        {
            var list = await GetBlobListAsync();
            return list.Where(i => !string.IsNullOrEmpty(i.Folder))
                       .Select(i => i.Folder)
                       .Distinct()
                       .OrderBy(i => i)
                       .ToList();
        }

        public async Task UploadAsync(string blobName, string filePath)
        {
            CloudBlockBlob blockBlob = await GetBlockBlobAsync(blobName);

            using (var fileStream = System.IO.File.Open(filePath, FileMode.Open))
            {
                fileStream.Position = 0;
                await blockBlob.UploadFromStreamAsync(fileStream);
            }
            File.Delete(filePath);
        }

        public async Task UploadAsync(string blobName, Stream stream)
        {
            CloudBlockBlob blockBlob = await GetBlockBlobAsync(blobName);

            stream.Position = 0;
            await blockBlob.UploadFromStreamAsync(stream);
        }

        public async Task<MemoryStream> DownloadAsync(string blobName)
        {
            CloudBlockBlob blockBlob = await GetBlockBlobAsync(blobName);

            using (var stream = new MemoryStream())
            {
                await blockBlob.DownloadToStreamAsync(stream);
                return stream;
            }
        }

        public async Task DeleteAsync(string blobName)
        {
            CloudBlockBlob blockBlob = await GetBlockBlobAsync(blobName);
            await blockBlob.DeleteIfExistsAsync();
        }


        public async Task UpdateFileNames(IEnumerable<string> from, IEnumerable<string> to)
        {
            using(var e1 = from.GetEnumerator())
            using(var e2 = to.GetEnumerator())
            {
                while(e1.MoveNext() && e2.MoveNext())
                {
                    await UpdateBlobName(e2.Current,e1.Current);
                }
            }
        }

        private async Task UpdateBlobName(string newBlobName, string oldBlobName)
        {
            CloudBlockBlob blockCopy = await GetBlockBlobAsync(newBlobName);
            if (!await blockCopy.ExistsAsync())  
            {  
                CloudBlockBlob blob = await GetBlockBlobAsync(oldBlobName);

                if (await blob.ExistsAsync())  
                {  
                    await blockCopy.StartCopyAsync(blob);  
                    await blob.DeleteIfExistsAsync();  
                } 
            } 

        }

        public async Task DownloadAsync(string blobName, string path)
        {
            CloudBlockBlob blockBlob = await GetBlockBlobAsync(blobName);

            await blockBlob.DownloadToFileAsync(path, FileMode.Create);
        }

        private async Task<CloudBlobContainer> GetContainerAsync()
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(new StorageCredentials(_blobStorageSettings.StorageAccount, _blobStorageSettings.StorageKey), false);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer blobContainer = blobClient.GetContainerReference(_blobStorageSettings.ContainerName);
            await blobContainer.CreateIfNotExistsAsync();

            return blobContainer;
        }

        private async Task<CloudBlockBlob> GetBlockBlobAsync(string blobName)
        {
            CloudBlobContainer blobContainer = await GetContainerAsync();

            CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(blobName);

            return blockBlob;
        }

        private async Task<List<AzureBlobItem>> GetBlobListAsync(bool useFlatListing = true)
        {
            CloudBlobContainer blobContainer = await GetContainerAsync();

            var list = new List<AzureBlobItem>();
            BlobContinuationToken token = null;
            do
            {
                BlobResultSegment resultSegment = await blobContainer.ListBlobsSegmentedAsync("", useFlatListing, new BlobListingDetails(), null, token, null, null);
                token = resultSegment.ContinuationToken;

                foreach (IListBlobItem item in resultSegment.Results)
                {
                    list.Add(new AzureBlobItem(item));
                }

            } while (token != null);

            return list.OrderBy(i => i.Folder).ThenBy(i => i.Name).ToList();
        }
    }
}
