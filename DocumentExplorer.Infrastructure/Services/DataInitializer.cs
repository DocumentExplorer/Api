using System;
using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;

namespace DocumentExplorer.Infrastructure.Services
{
    public class DataInitializer : IDataInitializer, IService
    {
        private readonly IUserService _userService;
        private readonly IPermissionsService _permissionsService;
        public DataInitializer(IUserService userService, IPermissionsService permissionsService)
        {
            _userService = userService;
            _permissionsService = permissionsService;
        }
        public async Task SeedAsync()
        {
            await _userService.RegisterAsync($"admn","secret123", Roles.Admin);        
            await _permissionsService.IntializePermissions();
        }
    }
}