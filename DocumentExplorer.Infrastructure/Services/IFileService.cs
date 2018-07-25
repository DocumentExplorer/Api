using System;
using System.Collections.Generic;
using System.IO;
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
        Task<FileDto> GetFileAsync(Guid id);
        Task DeleteFileAsync(Guid id);
        Task<MemoryStream> GetFileStreamAsync(Guid id);
        Task<IEnumerable<FileDto>> GetAllFilesAsync();
    }
}