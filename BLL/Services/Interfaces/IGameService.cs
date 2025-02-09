using BusinessLogic.Dto.Game;
using BusinessLogic.Requests.Game;

namespace BusinessLogic.Services.Interfaces;

public interface IGameService
{
    Task<GameDto> AddAsync(GameCreateRequest request);

    Task DeleteByIdAsync(Guid id);

    Task DeleteByKeyAsync(string key);

    IQueryable<GameDto> GetAll();

    Task<GameDto> GetByIdAsync(Guid id);

    Task<GameDto> GetByKeyAsync(string key);

    IQueryable<GameDto> GetAllByGenreId(Guid id);

    Task UpdateAsync(GameUpdateRequest request);

    Task<byte[]> GetGameFileAsync(string key);

    string GetGameFileName(string key);

    Task<int> GetTotalGamesCountAsync();
}
