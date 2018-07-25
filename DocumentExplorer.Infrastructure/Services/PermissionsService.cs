using System;
using System.Threading.Tasks;
using AutoMapper;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.DTO;

namespace DocumentExplorer.Infrastructure.Services
{
    public class PermissionsService : IPermissionsService
    {
        private readonly IPermissionsRepository _permissionsRepository;
        private readonly IMapper _mapper;
        public PermissionsService(IPermissionsRepository permissionsRepository, IMapper mapper)
        {
            _permissionsRepository = permissionsRepository;
            _mapper = mapper;
        }
        public async Task<PermissionsDto> GetPermissionsAsync()
        {
            var permissions = await _permissionsRepository.GetAsync();
            return _mapper.Map<PermissionsDto>(permissions);
        }

        public async Task IntializePermissions()
            => await _permissionsRepository.AddAsync(
                new Permissions(Roles.User, Roles.User, Roles.User, Roles.User, Roles.User, Roles.User, Roles.User, Roles.User, Roles.User));

        public async Task UpdatePermissions(string cmr, string fvk, string fvp, string nip, string nota, string pp, string rk, string zk, string zp)
        {
            var permissions = await _permissionsRepository.GetAsync();
            permissions.SetPermissions(cmr, fvk, fvp, nip, nota, pp, rk, zk, zp);
            await _permissionsRepository.UpdateAsync(permissions);
        }
    }
}