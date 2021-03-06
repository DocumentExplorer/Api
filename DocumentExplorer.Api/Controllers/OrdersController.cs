﻿using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Orders;
using DocumentExplorer.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace DocumentExplorer.Api.Controllers
{
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService; 
        public OrdersController(ICommandDispatcher commandDispatcher, IOrderService orderService, IMemoryCache cache) : base(commandDispatcher, cache)
        {
            _orderService = orderService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody]AddOrder command)
        {
            command.CacheId = Guid.NewGuid();
            await DispatchAsync(command);
            var result = new
            {
                Id = Cache.Get(command.CacheId)
            };
            return Json(result);
        }

        [Authorize("complementerAndAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var orders = await _orderService.GetAllAsync();
            return Json(orders);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetAllByUserAsync()
        {
            var orders = await _orderService.GetAllByUserAsync(Username);
            return Json(orders);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(Guid id)
        {
            var order = await _orderService.GetAsync(id);
            return Json(order);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var command = new DeleteOrder
            {
                OrderId = id
            };
            await DispatchAsync(command);
            return NoContent();
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> EditAsync([FromBody]EditOrder command)
        {
            await DispatchAsync(command);
            return NoContent();
        }

        [Authorize]
        [HttpPut("requirements")]
        public async Task<IActionResult> SetRequirementsAsync([FromBody]SetRequirements command)
        {
            await DispatchAsync(command);
            return NoContent();
        }

        [Authorize]
        [HttpGet("lacks")]
        public async Task<IActionResult> GetLackingFilesAsync()
        {
            var command = new GetLackingFiles();
            command.LackingFilesId = Guid.NewGuid();
            await DispatchAsync(command);
            var result = Cache.Get(command.LackingFilesId);
            return Json(result);
        }
    }
}
