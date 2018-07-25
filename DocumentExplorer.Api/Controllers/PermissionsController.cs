using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Permissions;
using DocumentExplorer.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DocumentExplorer.Api.Controllers
{
    [Route("[controller]")]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionsService _permissionsService;
        public PermissionsController(ICommandDispatcher commandDispatcher, IMemoryCache cache,
            IPermissionsService permissionsService) : base(commandDispatcher, cache)
        {
            _permissionsService = permissionsService;
        }

        [Authorize("admin")]
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var permissions = await _permissionsService.GetPermissionsAsync();
            return Json(permissions);
        }

        [Authorize("admin")]
        [HttpPut]
        public async Task<IActionResult> SetPermissionsAsync([FromBody]SetPermissions command)
        {
            await DispatchAsync(command);
            return NoContent();
        }

    }
}