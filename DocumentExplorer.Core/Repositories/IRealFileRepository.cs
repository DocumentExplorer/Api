using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DocumentExplorer.Core.Repositories
{
    public interface IRealFileRepository
    {
        Task<MemoryStream> GetAsync(string source);
        Task AddAsync(string source, string destination);
        Task RemoveAsync(string path);
        Task UpdateFileNames(IEnumerable<string> from,IEnumerable<string> to);
    }
}