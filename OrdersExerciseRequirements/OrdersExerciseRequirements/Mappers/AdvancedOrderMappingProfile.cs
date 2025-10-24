using AutoMapper;
using OrdersExerciseRequirements.Mappers.Resolvers;
using OrdersExerciseRequirements.Models;
using OrdersExerciseRequirements.DTOs;
namespace OrdersExerciseRequirements.Mappers;

public class AdvancedOrderMappingProfile : Profile
{
    public AdvancedOrderMappingProfile()
    {
        CreateMap<CreateOrderProfileRequest, Order>()
            .ForMember(d => d.IsAvailable, opt => opt.MapFrom(s => s.StockQuantity > 0));

        
        CreateMap<Order, OrderProfileDto>()
            .ForMember(d => d.Category, opt => opt.MapFrom<CategoryDisplayResolver>())
            .ForMember(d => d.Price,      opt => opt.MapFrom<PriceFormatterResolver>())
            .ForMember(d => d.PublishedAge,        opt => opt.MapFrom<PublishedAgeResolver>())
            .ForMember(d => d.AuthorInitials,      opt => opt.MapFrom<AuthorInitialsResolver>())
            .ForMember(d => d.AvailabilityStatus,  opt => opt.MapFrom<AvailabilityStatusResolver>());
    }
}