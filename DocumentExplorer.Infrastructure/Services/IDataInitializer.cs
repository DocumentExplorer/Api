using System.Threading.Tasks;

namespace DocumentExplorer.Infrastructure.Services
{
    public interface IDataInitializer
    {
        Task SeedAsync();
    }
}