using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Orders;
using DocumentExplorer.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DocumentExplorer.Api.Controllers
{
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService; 
        public OrdersController(ICommandDispatcher commandDispatcher, IOrderService orderService) : base(commandDispatcher)
        {
            _orderService = orderService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody]AddOrder command)
        {
            await DispatchAsync(command);
            return Created($"orders/{command.Id}", null);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var orders = await _orderService.GetAllAsync();
            return Json(orders);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var order = await _orderService.GetAsync(id);
            return Json(order);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
             var order = await _orderService.GetAsync(id);
            if((!IsAuthorized(order.Owner1Name)) && (!IsAuthorized(order.Owner2Name)))
            {
                return StatusCode(403);
            }
            await _orderService.DeleteAsync(id);
            return NoContent();
        }
    }
}
