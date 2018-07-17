using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;

namespace DocumentExplorer.Infrastructure.Services
{
    public class DataInitializer : IDataInitializer
    {
        private readonly IUserService _userService;
        public DataInitializer(IUserService userService)
        {
            _userService = userService;
        }
        public async Task SeedAsync()
        {
            var tasks = new List<Task>();
            for(int i=1; i<=3; i++)
            {
                tasks.Add(_userService.RegisterAsync($"usr{i}","secret123",Roles.User));
            }
            for(int i=1; i<=3; i++)
            {
                tasks.Add(_userService.RegisterAsync($"adm{i}","secret123",Roles.Admin));
            }
            await Task.WhenAll(tasks);
        }
    }
}