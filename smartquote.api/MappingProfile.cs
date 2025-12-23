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
        CreateMap<RegisterRequestDto, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

        CreateMap<LoginRequestDto, User>();

        // Item mappings
        CreateMap<CreateQuoteItemRequestDto, Item>().ReverseMap();
        CreateMap<UpdateQuoteItemRequestDto, Item>().ReverseMap();
        CreateMap<Item, ItemDto>().ReverseMap();

        // Quote mappings
        CreateMap<CreateQuoteRequestDto, Quote>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Total, opt => opt.Ignore());

        CreateMap<UpdateQuoteRequestDto, Quote>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.Total, opt => opt.Ignore());

        // 👇 This ensures Quote.Items → QuoteDto.Items is mapped automatically
        CreateMap<Quote, QuoteDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
    }
}
