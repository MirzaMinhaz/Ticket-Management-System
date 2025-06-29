namespace TMS.Application.Interfaces.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangeAsync();
    }
}