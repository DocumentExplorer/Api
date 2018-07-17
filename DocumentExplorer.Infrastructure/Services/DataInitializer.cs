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
            for(int i=1; i<=3; i++)
            {
                await _userService.RegisterAsync($"usr{i}","secret123",Roles.User);
            }
            for(int i=1; i<=3; i++)
            {
                await _userService.RegisterAsync($"adm{i}","secret123",Roles.Admin);
            }
        }
    }
}