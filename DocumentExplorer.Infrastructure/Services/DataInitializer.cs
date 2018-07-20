using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;

namespace DocumentExplorer.Infrastructure.Services
{
    public class DataInitializer : IDataInitializer
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        public DataInitializer(IUserService userService, IOrderService orderService)
        {
            _userService = userService;
            _orderService = orderService;
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
            var random = new Random();
            for (int i = 1; i <= 3; i++)
            {
                await _orderService.AddOrderAsync(i, "PL", random.Next(1, 100000).ToString(), "PL", random.Next(1, 100000).ToString(), $"usr{i}");
            }
            for (int i = 1; i <= 3; i++)
            {
                await _orderService.AddOrderAsync(i+3, "PL", random.Next(1, 100000).ToString(), "PL", random.Next(1, 100000).ToString(), $"adm{i}");
            }
        }
    }
}