using AutoMapper;
using BusinessLogic.Dto.Genre;
using BusinessLogic.Requests.Genre;
using DataAccess.Entities;

namespace BusinessLogic.Mappings;

public class GenreProfile : Profile
{
    public GenreProfile()
    {
        CreateMap<Genre, GenreDto>().ReverseMap();

        CreateMap<GenreCreateRequest, Genre>()
            .ForMember(g => g.Name, opt => opt.MapFrom(req => req.Genre.Name))
            .ForMember(g => g.ParentGenreId, opt => opt.MapFrom(req => req.Genre.ParentGenreId));

        CreateMap<GenreUpdateRequest, Genre>()
            .ForMember(g => g.Id, opt => opt.MapFrom(req => req.Genre.Id))
            .ForMember(g => g.Name, opt => opt.MapFrom(req => req.Genre.Name))
            .ForMember(g => g.ParentGenreId, opt => opt.MapFrom(req => req.Genre.ParentGenreId));
    }
}
