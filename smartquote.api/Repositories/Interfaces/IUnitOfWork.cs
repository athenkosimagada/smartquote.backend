namespace smartquote.api.Repositories.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    IUserRepository Users { get; }
    IQuoteRepository Quotes { get; }
    IQuoteItemRepository QuoteItems { get; }
    Task<int> SaveChangesAsync();
}
