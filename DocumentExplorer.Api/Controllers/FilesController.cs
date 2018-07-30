using System;
using System.IO;
using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Files;
using DocumentExplorer.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DocumentExplorer.Api.Controllers
{
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IFileService _fileService;
        public FilesController(ICommandDispatcher commandDispatcher, IMemoryCache cache, 
            IOrderService orderService, IFileService fileService) : base(commandDispatcher, cache)
        {
            _orderService = orderService;
            _fileService = fileService;
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> PostAsync(IFormFile file)
        {
            
            var command = new UploadFile();
            command.File = file;
            command.Id = Guid.NewGuid();
            await DispatchAsync(command);
            var result = new
            {
                Location = $"files/{command.Id}"
            };
            return Json(result);
        }

        [Authorize]
        [HttpPut("{uploadId}")]
        public async Task<IActionResult> PutIntoLocationAsync([FromBody]PutIntoLocation command, Guid uploadId)
        {
            command.UploadId = uploadId;
            await DispatchAsync(command);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{uploadId}")]
        public async Task<IActionResult> DeleteFile(Guid uploadId)
        {
            var file = await _fileService.GetFileAsync(uploadId);
            var order = await _orderService.GetAsync(file.OrderId);
            if(!IsAuthorizedPlusComplementer(order.Owner1Name))
            {
                return StatusCode(403);
            }
            await _fileService.DeleteFileAsync(uploadId, Role, Username);
            return NoContent();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFileAsync(Guid id)
        {
            var file = await _fileService.GetFileAsync(id);
            var order = await _orderService.GetAsync(file.OrderId);
            if(!IsAuthorizedPlusComplementer(order.Owner1Name))
            {
                return StatusCode(403);
            }
            var fileStream = await _fileService.GetFileStreamAsync(id);
            return new FileContentResult(fileStream.ToArray(),"application/pdf");
        }

        [Authorize("admin")]
        [HttpGet("metadata/{id}")]
        public async Task<IActionResult> GetFileMetaDataAsync(Guid id)
        {
            var file = await _fileService.GetFileAsync(id);
            return Json(file);
        }

        [Authorize("admin")]
        [HttpGet("metadata")]
        public async Task<IActionResult> GetFileMetaDatasAsync()
        {
            var files = await _fileService.GetAllFilesAsync();
            return Json(files);
        }

        [Authorize("admin")]
        [HttpGet("generate")]
        public async Task<IActionResult> GenerateAsync()
        {
            await _fileService.GenerateAsync();
            return NoContent();
        } 

    }
}