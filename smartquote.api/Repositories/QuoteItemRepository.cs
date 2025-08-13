using smartquote.api.Data;
using smartquote.api.Entities;
using smartquote.api.Repositories.Interfaces;

namespace smartquote.api.Repositories;

public class QuoteItemRepository : Repository<Item>, IQuoteItemRepository
{
    public QuoteItemRepository(SmartQuoteDbContext context)
        : base(context) { }
    public void Update(Item item)
    {
        SmartQuoteDbContext.Items.Update(item);
    }

    public SmartQuoteDbContext SmartQuoteDbContext
    {
        get {
            return _context as SmartQuoteDbContext
                ?? throw new InvalidOperationException("Context is not SmartQuoteDbContext"); }
    }
}
