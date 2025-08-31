using Microsoft.EntityFrameworkCore;
using smartquote.api.Data;
using smartquote.api.Entities;
using smartquote.api.Repositories.Interfaces;

namespace smartquote.api.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(SmartQuoteDbContext context)
        : base(context) { }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await SmartQuoteDbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public void Update(User user)
    {
        SmartQuoteDbContext.Users.Update(user);
    }

    public SmartQuoteDbContext SmartQuoteDbContext
    {
        get {
            return _context as SmartQuoteDbContext
                ?? throw new InvalidOperationException("Context is not SmartQuoteDbContext"); }
    }
}
