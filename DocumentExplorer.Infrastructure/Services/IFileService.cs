using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DocumentExplorer.Infrastructure.Services
{
    public interface IFileService
    {
        Task UploadAsync(IFormFile file, Guid id);
        void Validate(IFormFile file);
        Task PutIntoLocationAsync(Guid uploadId, Guid orderId, string fileType, int invoiceNumber);
    }
}