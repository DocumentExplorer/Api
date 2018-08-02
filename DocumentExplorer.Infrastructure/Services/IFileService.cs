using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DocumentExplorer.Infrastructure.Services
{
    public interface IFileService
    {
        Task UploadAsync(IFormFile file, Guid id);
        void Validate(IFormFile file);
        Task PutIntoLocationAsync(Guid uploadId, Guid orderId, string fileType, 
            int invoiceNumber, string role, string username);
        Task DeleteFileAsync(Guid id, string fileType, string role, string username);
        Task<MemoryStream> GetFileStreamAsync(Guid orderId, string fileType);
        Task GenerateAsync();
    }
}