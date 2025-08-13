using smartquote.api.Data;
using smartquote.api.Repositories.Interfaces;

namespace smartquote.api.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly SmartQuoteDbContext _context;
    public UnitOfWork(SmartQuoteDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
    }

    public IUserRepository Users { get; private set; }

    public async Task<int> SaveChangesAsync()
    {
        return  await _context.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
       await _context.DisposeAsync();
    }
}
