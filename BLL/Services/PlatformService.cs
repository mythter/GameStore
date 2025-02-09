using AutoMapper;
using BusinessLogic.Dto.Platform;
using BusinessLogic.Exceptions;
using BusinessLogic.Requests.Platform;
using BusinessLogic.Services.Interfaces;
using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;
using DataAccess.UnitOfWork;

namespace BusinessLogic.Services;
public class PlatformService(
    IUnitOfWork unitOfWork,
    IMapper mapper) : IPlatformService
{
    private IPlatformRepository PlatformRepository => unitOfWork.PlatformRepository;

    private IGameRepository GameRepository => unitOfWork.GameRepository;

    private IGamePlatformRepository GamePlatformRepository => unitOfWork.GamePlatformRepository;

    public async Task<PlatformDto> AddAsync(PlatformCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Platform.Type))
        {
            throw new BadRequestException("Platform type cannot be null or empty.");
        }

        var platform = mapper.Map<Platform>(request);

        var entity = await PlatformRepository.AddAsync(platform);

        await unitOfWork.SaveChangesAsync();

        return mapper.Map<PlatformDto>(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await PlatformRepository.GetByIdAsync(id);
        if (entity is not null)
        {
            PlatformRepository.Delete(entity);
            await unitOfWork.SaveChangesAsync();
        }
    }

    public IQueryable<PlatformDto> GetAll()
    {
        var entities = PlatformRepository.GetAll();
        return mapper.ProjectTo<PlatformDto>(entities);
    }

    public async Task<IQueryable<PlatformDto>> GetAllByGameKey(string key)
    {
        var gameEntity = await GameRepository.GetByKeyAsync(key)
            ?? throw new NotFoundException($"Game with key {key} was not found.");

        var gamePlatforms =
            from gp in GamePlatformRepository.GetAll()
            where gp.GameId == gameEntity.Id
            join game in GameRepository.GetAll() on gp.GameId equals game.Id
            join platform in PlatformRepository.GetAll() on gp.PlatformId equals platform.Id
            select platform;

        return mapper.ProjectTo<PlatformDto>(gamePlatforms);
    }

    public async Task<PlatformDto> GetByIdAsync(Guid id)
    {
        var entity = await PlatformRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Platform with id {id} was not found.");

        return mapper.Map<PlatformDto>(entity);
    }

    public async Task UpdateAsync(PlatformUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Platform.Type))
        {
            throw new BadRequestException("Platform type cannot be null or empty.");
        }

        var entity = await PlatformRepository.GetByIdAsync(request.Platform.Id)
            ?? throw new NotFoundException($"Platform with id {request.Platform.Id} was not found.");

        mapper.Map(request.Platform, entity);

        await unitOfWork.SaveChangesAsync();
    }
}
