using System;
using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Infrastructure.DTO;

namespace DocumentExplorer.Infrastructure.Services
{
    public interface IPermissionsService
    {
        Task<PermissionsDto> GetPermissionsAsync();
        Task IntializePermissions();
        Task UpdatePermissions(string cmr, 
            string fvk, string fvp, string nip, string nota,
            string pp, string rk, string zk, string zp);
        Task Validate(string fileType, string role);
        Task<Permissions> GetPermissionsObjectAsync();
    }
}