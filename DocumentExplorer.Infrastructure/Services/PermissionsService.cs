using System;
using System.Threading.Tasks;
using AutoMapper;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.DTO;
using DocumentExplorer.Infrastructure.Exceptions;

namespace DocumentExplorer.Infrastructure.Services
{
    public class PermissionsService : IPermissionsService, IService
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

        public async Task<Permissions> GetPermissionsObjectAsync()
            => await _permissionsRepository.GetAsync();

        public async Task IntializePermissions()
            => await _permissionsRepository.AddAsync(
                new Permissions(Roles.User, Roles.User, Roles.User, Roles.User, Roles.User, Roles.User, Roles.User, Roles.User, Roles.User));

        public async Task UpdatePermissions(string cmr, string fvk, string fvp, string nip, string nota, string pp, string rk, string zk, string zp)
        {
            var permissions = await _permissionsRepository.GetAsync();
            permissions.SetPermissions(cmr, fvk, fvp, nip, nota, pp, rk, zk, zp);
            await _permissionsRepository.UpdateAsync(permissions);
        }

        public async Task Validate(string fileType, string role)
        {
            var permissions = await _permissionsRepository.GetAsync();
            var properties = typeof(FileTypes).GetProperties();
            foreach(var property in properties)
            {
                if(property.Name.ToLower()==fileType)
                {
                    if(!(GetPermissionsFileTypePropertyValue(permissions, property.Name)==role 
                        || role == Roles.Admin))
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
            }
        }

        private string GetPermissionsFileTypePropertyValue(Permissions permissions, string propertyName)
        {
            var propertyObject = typeof(Permissions).GetProperty(propertyName).GetValue(permissions, null);
            if(propertyObject is string result)
            {
                return result;
            }
            throw new InvalidCastException();
        }
    }
}