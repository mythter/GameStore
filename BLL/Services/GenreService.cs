using AutoMapper;
using BusinessLogic.Dto.Genre;
using BusinessLogic.Exceptions;
using BusinessLogic.Requests.Genre;
using BusinessLogic.Services.Interfaces;
using DataAccess.Entities;
using DataAccess.Repositories.Interfaces;
using DataAccess.UnitOfWork;

namespace BusinessLogic.Services;
public class GenreService(
    IUnitOfWork unitOfWork,
    IMapper mapper) : IGenreService
{
    private IGameRepository GameRepository => unitOfWork.GameRepository;

    private IGenreRepository GenreRepository => unitOfWork.GenreRepository;

    private IGameGenreRepository GameGenreRepository => unitOfWork.GameGenreRepository;

    public async Task<GenreDto> AddAsync(GenreCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Genre.Name))
        {
            throw new BadRequestException("Genre name cannot be null or empty.");
        }

        var genre = mapper.Map<Genre>(request);

        var entity = await GenreRepository.AddAsync(genre);

        await unitOfWork.SaveChangesAsync();

        return mapper.Map<GenreDto>(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GenreRepository.GetByIdAsync(id);
        if (entity is not null)
        {
            GenreRepository.Delete(entity);
            await unitOfWork.SaveChangesAsync();
        }
    }

    public IQueryable<GenreDto> GetAll()
    {
        var entities = GenreRepository.GetAll();
        return mapper.ProjectTo<GenreDto>(entities);
    }

    public async Task<IQueryable<GenreDto>> GetAllByGameKey(string key)
    {
        var gameEntity = await GameRepository.GetByKeyAsync(key)
            ?? throw new NotFoundException($"Game with key {key} was not found.");

        var gameGenres =
            from gg in GameGenreRepository.GetAll()
            where gg.GameId == gameEntity.Id
            join game in GameRepository.GetAll() on gg.GameId equals game.Id
            join genre in GenreRepository.GetAll() on gg.GenreId equals genre.Id
            select genre;

        return mapper.ProjectTo<GenreDto>(gameGenres);
    }

    public IQueryable<GenreDto> GetAllByParentId(Guid id)
    {
        var entities = GenreRepository.GetAll().Where(g => g.ParentGenreId == id);
        return mapper.ProjectTo<GenreDto>(entities);
    }

    public async Task<GenreDto> GetByIdAsync(Guid id)
    {
        var entity = await GenreRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Genre with id {id} was not found.");

        return mapper.Map<GenreDto>(entity);
    }

    public async Task UpdateAsync(GenreUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Genre.Name))
        {
            throw new BadRequestException("Genre name cannot be null or empty.");
        }

        var entity = await GenreRepository.GetByIdAsync(request.Genre.Id)
            ?? throw new NotFoundException($"Genre with id {request.Genre.Id} was not found.");

        mapper.Map(request.Genre, entity);

        await unitOfWork.SaveChangesAsync();
    }
}
