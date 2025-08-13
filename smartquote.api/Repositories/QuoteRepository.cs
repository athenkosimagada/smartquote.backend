using smartquote.api.Data;
using smartquote.api.Entities;
using smartquote.api.Repositories.Interfaces;

namespace smartquote.api.Repositories;

public class QuoteRepository : Repository<Quote>, IQuoteRepository
{
    public QuoteRepository(SmartQuoteDbContext context)
        : base(context) { }
    public void Update(Quote quote)
    {
        SmartQuoteDbContext.Quotes.Update(quote);
    }
    public SmartQuoteDbContext SmartQuoteDbContext
    {
        get {
            return _context as SmartQuoteDbContext
                ?? throw new InvalidOperationException("Context is not SmartQuoteDbContext"); }
    }
}
