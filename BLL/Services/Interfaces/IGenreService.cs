using BusinessLogic.Dto.Genre;
using BusinessLogic.Requests.Genre;

namespace BusinessLogic.Services.Interfaces;

public interface IGenreService
{
    Task<GenreDto> AddAsync(GenreCreateRequest request);

    Task DeleteAsync(Guid id);

    IQueryable<GenreDto> GetAll();

    Task<GenreDto> GetByIdAsync(Guid id);

    IQueryable<GenreDto> GetAllByParentId(Guid id);

    Task<IQueryable<GenreDto>> GetAllByGameKey(string key);

    Task UpdateAsync(GenreUpdateRequest request);
}
