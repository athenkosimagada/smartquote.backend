namespace smartquote.api.Repositories.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    IUserRepository Users { get; }
    Task<int> SaveChangesAsync();
}
