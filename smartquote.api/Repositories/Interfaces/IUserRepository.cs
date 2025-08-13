using smartquote.api.Entities;

namespace smartquote.api.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    void Update(User user);
}
