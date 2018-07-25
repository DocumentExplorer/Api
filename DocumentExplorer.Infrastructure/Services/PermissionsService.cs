using System;
using System.Threading.Tasks;
using AutoMapper;
using DocumentExplorer.Core.Domain;
using DocumentExplorer.Core.Repositories;
using DocumentExplorer.Infrastructure.DTO;
using DocumentExplorer.Infrastructure.Exceptions;

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

        public async Task Validate(string fileType, string role)
        {
            var permissions = await _permissionsRepository.GetAsync();
            switch(fileType)
            {
                case "cmr":
                    if(!(permissions.CMR==role || role == Roles.Admin)) 
                        throw new UnauthorizedAccessException();
                    break;
                case "fvk":
                    if(!(permissions.FVK==role || role == Roles.Admin)) 
                        throw new UnauthorizedAccessException();
                    break;
                case "fvp":
                    if(!(permissions.FVP==role || role == Roles.Admin)) 
                        throw new UnauthorizedAccessException();
                    break;
                case "nip":
                    if(!(permissions.NIP==role || role == Roles.Admin)) 
                        throw new UnauthorizedAccessException();
                    break;
                case "nota":
                    if(!(permissions.Nota==role || role == Roles.Admin)) 
                        throw new UnauthorizedAccessException();
                    break;
                case "pp":
                    if(!(permissions.PP==role || role == Roles.Admin)) 
                        throw new UnauthorizedAccessException();
                    break;
                case "rk":
                    if(!(permissions.RK==role || role == Roles.Admin)) 
                        throw new UnauthorizedAccessException();
                    break;
                case "zk":
                    if(!(permissions.ZK==role || role == Roles.Admin)) 
                        throw new UnauthorizedAccessException();
                    break;
                case "zp":
                    if(!(permissions.ZP==role || role == Roles.Admin)) 
                        throw new UnauthorizedAccessException();
                    break;
                default:
                    throw new ServiceException(Exceptions.ErrorCodes.InvalidFileType);
            }
        }
    }
}