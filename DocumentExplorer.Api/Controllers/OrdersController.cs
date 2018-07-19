using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DocumentExplorer.Api.Controllers
{
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        public OrdersController(ICommandDispatcher commandDispatcher) : base(commandDispatcher)
        {

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody]AddOrder command)
        {
            await DispatchAsync(command);
            return Created($"orders/{command.Id}", null);
        }
    }
}
