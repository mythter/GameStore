using AutoMapper;
using BusinessLogic.Dto.Game;
using BusinessLogic.Requests.Game;
using DataAccess.Entities;

namespace BusinessLogic.Mappings;

public class GameProfile : Profile
{
    public GameProfile()
    {
        CreateMap<Game, GameDto>().ReverseMap();

        CreateMap<GameCreateRequest, Game>()
            .ForMember(g => g.Name, opt => opt.MapFrom(req => req.Game.Name))
            .ForMember(g => g.Key, opt => opt.MapFrom(req => req.Game.Key))
            .ForMember(g => g.Description, opt => opt.MapFrom(req => req.Game.Description));

        CreateMap<GameUpdateRequest, Game>()
            .ForMember(g => g.Id, opt => opt.MapFrom(req => req.Game.Id))
            .ForMember(g => g.Name, opt => opt.MapFrom(req => req.Game.Name))
            .ForMember(g => g.Key, opt => opt.MapFrom(req => req.Game.Key))
            .ForMember(g => g.Description, opt => opt.MapFrom(req => req.Game.Description));
    }
}
