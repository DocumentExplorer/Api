using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.DTO;
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
        private readonly IPermissionsService _permissionService;
        private readonly IMapper _mapper;
        public FileService(IFileRepository fileRepository, IOrderRepository orderRepository, 
            IRealFileRepository realFileRepository, IMapper mapper, IPermissionsService permissionService)
        {
            _fileRepository = fileRepository;
            _orderRepository = orderRepository;
            _realFileRepository = realFileRepository;
            _mapper = mapper;
            _permissionService = permissionService;
        }

        public async Task DeleteFileAsync(Guid id, string role)
        {
            var file = await _fileRepository.GetOrFailAsync(id);
            await _permissionService.Validate(file.FileType, role);
            var order = await _orderRepository.GetOrFailAsync(file.OrderId);
            await _realFileRepository.RemoveAsync(file.Path);
            await _fileRepository.RemoveAsync(file);
            order.UnlinkFile(file.FileType);
            await _orderRepository.UpdateAsync(order);
        }

        public async Task<IEnumerable<FileDto>> GetAllFilesAsync()
        {
            var files = await _fileRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<FileDto>>(files);
        }

        public async Task<FileDto> GetFileAsync(Guid id)
        {
            var file = await _fileRepository.GetOrFailAsync(id);
            return _mapper.Map<FileDto>(file);
        }

        public async Task<MemoryStream> GetFileStreamAsync(Guid id)
        {
            var file = await _fileRepository.GetOrFailAsync(id);
            return await _realFileRepository.GetOrFailAsync(file.Path);
        }

        public async Task PutIntoLocationAsync(Guid uploadId, Guid orderId, string fileType, int invoiceNumber, string role)
        {
            await _permissionService.Validate(fileType, role);
            var file = await _fileRepository.GetOrFailAsync(uploadId);
            var order = await _orderRepository.GetOrFailAsync(orderId);
            file.SetOrderId(order.Id);
            file.SetFileType(fileType);
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
            var fileData = new Core.Domain.File(id,filePath, Guid.Empty);
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