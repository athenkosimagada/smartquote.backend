using smartquote.api.Entities;

namespace smartquote.api.Repositories.Interfaces;

public interface IQuoteRepository : IRepository<Quote>
{
    void Update(Quote quote);
    Task<int> TotalQuoteCount();
}
