using AutoMapper;
using smartquote.api.DTOs.Account;
using smartquote.api.DTOs.Items;
using smartquote.api.DTOs.Items.Requests;
using smartquote.api.DTOs.Quotes;
using smartquote.api.DTOs.Quotes.Requests;
using smartquote.api.Entities;

namespace smartquote.api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, AccountDetailsDto>().ReverseMap();
        CreateMap<RegisterRequestDto, User>();
        CreateMap<LoginRequestDto, User>();
        CreateMap<CreateQuoteItemRequestDto, Item>().ReverseMap();
        CreateMap<UpdateQuoteItemRequestDto, Item>().ReverseMap();
        CreateMap<Item, ItemDto>().ReverseMap();
        CreateMap<CreateQuoteRequestDto, Quote>().ReverseMap();
        CreateMap<UpdateQuoteRequestDto, Quote>().ReverseMap();
        CreateMap<Quote, QuoteDto>();
    }
}
