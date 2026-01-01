using AutoMapper;
using smartquote.api.DTOs.Quotes;
using smartquote.api.DTOs.Quotes.Requests;
using smartquote.api.DTOs.Quotes.Responses;
using smartquote.api.Entities;
using smartquote.api.Exceptions;
using smartquote.api.Repositories.Interfaces;
using smartquote.api.Services.Interfaces;

namespace smartquote.api.Services;

public class QuoteService : IQuoteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public QuoteService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<QuoteResponseDto> GetQuoteByIdAsync(int id)
    {
        var quote = await _unitOfWork.Quotes.GetByIdAsync(id, includeItems: true);
        if(quote == null)
        {
            throw new NotFoundException($"Quote with ID {id} not found.");
        }

        return new QuoteResponseDto
        {
            Quote = _mapper.Map<QuoteDto>(quote),
        };
    }

    public async Task<QuotesResponseDto> GetQuotesAsync(int pageNumber, int pageSize)
    {
        var quotes = await _unitOfWork.Quotes.GetAllAsync(pageNumber, pageSize, includeItems: true);

        return new QuotesResponseDto
        {
            Quotes = _mapper.Map<List<QuoteDto>>(quotes),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = await _unitOfWork.Quotes.TotalQuoteCount(),
        };
    }

    public async Task<CreateQuoteResponseDto> CreateQuoteAsync(string userId, CreateQuoteRequestDto request)
    {
        var quote = _mapper.Map<Quote>(request);

        quote.UserId = userId;
        quote.Total = quote.Items.Sum(i => i.Price * i.Quantity);

        await _unitOfWork.Quotes.AddAsync(quote);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result <= 0)
        {
            throw new Exception("Failed to create the quote.");
        }
        var response = new CreateQuoteResponseDto();
        response.QuoteId = quote.Id;
        return response;
    }

    public async Task<UpdateQuoteResponseDto> UpdateQuoteAsync(string userId, UpdateQuoteRequestDto request)
    {
        var quote = await _unitOfWork.Quotes.GetByIdAsync(request.Id);
        if (quote == null)
        {
            throw new NotFoundException($"Quote with ID {request.Id} not found.");
        }

        quote.CustomerName = request.CustomerName;
        quote.Items.Clear();

        foreach (var itemDto in request.Items)
        {
            var item = _mapper.Map<Item>(itemDto);
            item.QuoteId = quote.Id;
            quote.Items.Add(item);
        }

        quote.Total = quote.Items.Sum(i => i.Price * i.Quantity);

        _unitOfWork.Quotes.Update(quote);

        var result = await _unitOfWork.SaveChangesAsync();
        if (result <= 0)
        {
            throw new Exception("Failed to update the quote.");
        }

        var response = new UpdateQuoteResponseDto();
        response.QuoteId = quote.Id;
        return response;
    }

    public async Task DeleteQuoteAsync(int id)
    {
        var quote = await _unitOfWork.Quotes.GetByIdAsync(id);

        if (quote == null)
        {
            throw new NotFoundException($"Quote with ID {id} not found.");
        }

        _unitOfWork.Quotes.Remove(quote);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result <= 0)
        {
            throw new Exception("Failed to delete the quote.");
        }
    }
}
