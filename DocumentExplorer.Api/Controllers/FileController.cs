using System.IO;
using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DocumentExplorer.Api.Controllers
{
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        public FileController(ICommandDispatcher commandDispatcher, IMemoryCache cache) : base(commandDispatcher, cache)
        {

        }

        [HttpPost("upload")]
        public async Task<IActionResult> Post(IFormFile file)
        {
            var filePath = Path.GetTempFileName();

            if(file.Length > 0)
            {
                using(var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            return Created(filePath, null);
        }

    }
}