using BusinessLogic.Dto.Platform;
using BusinessLogic.Requests.Platform;

namespace BusinessLogic.Services.Interfaces;

public interface IPlatformService
{
    Task<PlatformDto> AddAsync(PlatformCreateRequest request);

    Task DeleteAsync(Guid id);

    Task<PlatformDto> GetByIdAsync(Guid id);

    IQueryable<PlatformDto> GetAll();

    Task<IQueryable<PlatformDto>> GetAllByGameKey(string key);

    Task UpdateAsync(PlatformUpdateRequest request);
}
