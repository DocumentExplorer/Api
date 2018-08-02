namespace DocumentExplorer.Infrastructure.Services
{
    public interface ITokenManager
    {
        bool IsCurrentActive();
        void DeactivateCurrent();
        bool IsActive(string token);
        void Deactivate(string token);
        
    }
}