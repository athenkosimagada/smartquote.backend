using smartquote.api.Entities;

namespace smartquote.api.Repositories.Interfaces;

public interface IQuoteItemRepository : IRepository<Item>
{
    void Update(Item item);
}
