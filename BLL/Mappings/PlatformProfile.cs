using AutoMapper;
using BusinessLogic.Dto.Platform;
using BusinessLogic.Requests.Platform;
using DataAccess.Entities;

namespace BusinessLogic.Mappings;

public class PlatformProfile : Profile
{
    public PlatformProfile()
    {
        CreateMap<Platform, PlatformDto>().ReverseMap();

        CreateMap<PlatformCreateRequest, Platform>()
            .ForMember(g => g.Type, opt => opt.MapFrom(req => req.Platform.Type));

        CreateMap<PlatformUpdateRequest, Platform>()
            .ForMember(g => g.Id, opt => opt.MapFrom(req => req.Platform.Id))
            .ForMember(g => g.Type, opt => opt.MapFrom(req => req.Platform.Type));
    }
}
