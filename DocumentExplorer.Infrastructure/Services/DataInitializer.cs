using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;

namespace DocumentExplorer.Infrastructure.Services
{
    public class DataInitializer : IDataInitializer, IService
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly IPermissionsService _permissionsService;
        public DataInitializer(IUserService userService, IOrderService orderService, IPermissionsService permissionsService)
        {
            _userService = userService;
            _orderService = orderService;
            _permissionsService = permissionsService;
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
            for(int i=1; i<=3; i++)
            {
                await _userService.RegisterAsync($"com{i}","secret123",Roles.Complementer);
            }
            var random = new Random();
            for (int i = 1; i <= 3; i++)
            {
                await _orderService.AddOrderAsync(Guid.NewGuid(),i, "PL", random.Next(1, 100000).ToString(), 
                    "PL", random.Next(1, 100000).ToString(), $"usr{i}", "admin");
            }
            for (int i = 1; i <= 3; i++)
            {
                await _orderService.AddOrderAsync(Guid.NewGuid(), i+3, "PL", random.Next(1, 100000).ToString(), 
                    "PL", random.Next(1, 100000).ToString(), $"adm{i}", "admin");
            }
            
            await _permissionsService.IntializePermissions();
        }
    }
}