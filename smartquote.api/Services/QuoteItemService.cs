using AutoMapper;
using smartquote.api.DTOs.Items;
using smartquote.api.DTOs.Items.Requests;
using smartquote.api.DTOs.Items.Responses;
using smartquote.api.Entities;
using smartquote.api.Exceptions;
using smartquote.api.Repositories.Interfaces;
using smartquote.api.Services.Interfaces;

namespace smartquote.api.Services;

public class QuoteItemService : IQuoteItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public QuoteItemService(
        IUnitOfWork unitOfWork, 
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<QuoteItemsResponseDto> GetAllQuoteItemsAsync(int pageNumber, int pageSize)
    {
        var quoteItems = await _unitOfWork.QuoteItems.GetAllAsync(pageNumber,pageSize);
        return new QuoteItemsResponseDto
        {
            Items = _mapper.Map<List<ItemDto>>(quoteItems),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = quoteItems.Count(),
        };
    }

    public async Task<QuoteItemResponseDto> GetQuoteItemByIdAsync(int id)
    {
        var quoteItem = await _unitOfWork.QuoteItems.GetByIdAsync(id);
        if (quoteItem == null)
        {
            throw new NotFoundException($"Quote item with ID {id} not found.");
        }
        return new QuoteItemResponseDto
        { 
            Item = _mapper.Map<ItemDto>(quoteItem)
        };
    }

    public async Task<CreateQuoteItemResponseDto> CreateQuoteItemAsync(CreateQuoteItemRequestDto request)
    {
        var quoteItem = _mapper.Map<Item>(request);

        await _unitOfWork.QuoteItems.AddAsync(quoteItem);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result <= 0)
        {
            throw new Exception("Failed to create the quote item.");
        }
        var response = new CreateQuoteItemResponseDto();
        response.ItemId = quoteItem.Id;
        return response;
    }

    public async Task<UpdateQuoteItemResponseDto> UpdateQuoteItemAsync(UpdateQuoteItemRequestDto request)
    {
        var quoteItem = await _unitOfWork.QuoteItems.GetByIdAsync(request.Id);
        if (quoteItem == null)
        {
            throw new NotFoundException($"Quote item with ID {request.Id} not found.");
        }

        _mapper.Map(request, quoteItem);
        _unitOfWork.QuoteItems.Update(quoteItem);

        var result = await _unitOfWork.SaveChangesAsync();
        if (result <= 0)
        {
            throw new Exception("Failed to update the quote item.");
        }
        
        var response =  new UpdateQuoteItemResponseDto();
        response.ItemId = quoteItem.Id;
        return response;
    }

    public async Task DeleteQuoteItemAsync(int id)
    {
        var quoteItem = await _unitOfWork.QuoteItems.GetByIdAsync(id);

        if (quoteItem == null)
        {
            throw new NotFoundException($"Quote item with ID {id} not found.");
        }

        _unitOfWork.QuoteItems.Remove(quoteItem);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result <= 0)
        {
            throw new Exception("Failed to delete the quote item.");
        }
    }
}
