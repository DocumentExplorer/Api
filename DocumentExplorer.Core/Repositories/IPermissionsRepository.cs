using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;

namespace DocumentExplorer.Core.Repositories
{
    public interface IPermissionsRepository
    {
        Task<Permissions> GetAsync();
        Task AddAsync(Permissions permissions);
        Task UpdateAsync(Permissions permissions);
    }
}