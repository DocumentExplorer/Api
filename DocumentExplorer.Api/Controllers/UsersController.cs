using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentExplorer.Infrastructure.Commands;
using DocumentExplorer.Infrastructure.Commands.Users;
using Microsoft.AspNetCore.Mvc;

namespace DocumentExplorer.Api.Controllers
{
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly ICommandDispatcher _commandDispatcher;
        public UsersController(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2", DateTime.Now.ToString() };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]Register command)
        {
            await _commandDispatcher.DispatchAsync(command);
            return Created($"users/{command.Username}", new object());
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
