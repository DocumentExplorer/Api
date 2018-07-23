using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentExplorer.Core.Domain;

namespace DocumentExplorer.Core.Repositories
{
    public interface IFileRepository
    {
        Task<File> GetAsync(Guid id);
        Task AddAsync(File file);
        Task RemoveAsync(File file);
        Task UpdateAsync(File file);
    }
}