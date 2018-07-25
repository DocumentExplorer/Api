using System;
using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.DTO;
using Microsoft.AspNetCore.Http;

namespace DocumentExplorer.Infrastructure.Services
{
    public interface IFileService
    {
        Task UploadAsync(IFormFile file, Guid id);
        void Validate(IFormFile file);
        Task PutIntoLocationAsync(Guid uploadId, Guid orderId, string fileType, int invoiceNumber);
        Task<FileDto> GetFile(Guid id);
        Task DeleteFile(Guid id);
    }
}