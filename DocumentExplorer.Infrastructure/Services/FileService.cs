using System;
using System.IO;
using System.Threading.Tasks;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.Exceptions;
using DocumentExplorer.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace DocumentExplorer.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IRealFileRepository _realFileRepository;
        public FileService(IFileRepository fileRepository, IOrderRepository orderRepository, 
            IRealFileRepository realFileRepository)
        {
            _fileRepository = fileRepository;
            _orderRepository = orderRepository;
            _realFileRepository = realFileRepository;
        }

        public async Task PutIntoLocationAsync(Guid uploadId, Guid orderId, string fileType, int invoiceNumber)
        {
            var file = await _fileRepository.GetOrFailAsync(uploadId);
            var order = await _orderRepository.GetOrFailAsync(orderId);
            order.LinkFile(file,fileType,invoiceNumber);
            var destination = order.GetPathToFile(fileType);
            await _realFileRepository.AddAsync(file.Path, destination);
            file.Path = destination;
            await _orderRepository.UpdateAsync(order);
            await _fileRepository.UpdateAsync(file);
        }

        public async Task UploadAsync(IFormFile file, Guid id)
        {
            var filePath = Path.GetTempFileName();

            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var fileData = new Core.Domain.File(id,filePath);
            await _fileRepository.AddAsync(fileData);
        }

        public void Validate(IFormFile file)
        {
            if(file == null)
            {
                throw new ServiceException(ErrorCodes.NoFile);
            }
            if(file.FileName == null)
            {
                throw new ServiceException(ErrorCodes.NoFileName);
            }
            if(!file.FileName.EndsWith(".pdf"))
            {
                throw new ServiceException(ErrorCodes.InvalidExtension);
            }
            if(file.Length <= 0)
            {
                throw new ServiceException(ErrorCodes.FileHasNoData);
            }
        }
    }
}