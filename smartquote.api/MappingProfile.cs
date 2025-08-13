using AutoMapper;
using smartquote.api.DTOs.Auth;
using smartquote.api.DTOs.Items;
using smartquote.api.DTOs.Items.Requests;
using smartquote.api.Entities;

namespace smartquote.api;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateQuoteItemRequestDto, Item>();
        CreateMap<UpdateQuoteItemRequestDto, Item>();
        CreateMap<Item, ItemDto>();
    }
}
