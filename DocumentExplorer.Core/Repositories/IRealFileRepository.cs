using System.Threading.Tasks;

namespace DocumentExplorer.Core.Repositories
{
    public interface IRealFileRepository
    {
        Task GetAsync(string source, string destination);
        Task AddAsync(string source, string destination);
        Task RemoveAsync(string path);
        Task UpdateFolderName(string from, string to);
    }
}