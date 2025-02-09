using AutoMapper;
using BusinessLogic.Dto.Genre;
using BusinessLogic.Exceptions;
using BusinessLogic.Mappings;
using BusinessLogic.Requests.Genre;
using BusinessLogic.Services;
using DataAccess.Entities;
using DataAccess.UnitOfWork;
using Moq;
using XUnitTests.Helpers;

namespace XUnitTests;

public class GenreServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;

    private readonly IMapper _mapper;

    public GenreServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        var config = new MapperConfiguration(cfg => { cfg.AddProfile<GenreProfile>(); });
        _mapper = config.CreateMapper();
    }

    #region Get tests

    [Fact]
    public async Task GetById_GenreDoesNotExist_ThrowNotFoundException()
    {
        // arrange
        Guid notExistedGameId = Guid.NewGuid();

        _mockUnitOfWork.Setup(x => x.GenreRepository.GetByIdAsync(notExistedGameId))
            .ReturnsAsync((Genre)null);

        var genreService = new GenreService(_mockUnitOfWork.Object, _mapper);

        // act & assert
        await Assert.ThrowsAsync<NotFoundException>(() => genreService.GetByIdAsync(notExistedGameId));
    }

    [Fact]
    public async Task GetById_GenreExists_ReturnGameDto()
    {
        // arrange
        Guid existedGenreId = Guid.NewGuid();
        var genre = new Genre() { Id = existedGenreId };

        _mockUnitOfWork.Setup(x => x.GenreRepository.GetByIdAsync(existedGenreId))
            .ReturnsAsync(genre);

        var genreService = new GenreService(_mockUnitOfWork.Object, _mapper);

        // act
        var result = await genreService.GetByIdAsync(existedGenreId);

        // assert
        Assert.NotNull(result);
        Assert.Equal(existedGenreId, result.Id);
        Assert.IsType<GenreDto>(result);
    }

    [Fact]
    public void GetAll_ThereAreNoGenres_ReturnEmptyList()
    {
        // arrange
        var genres = Enumerable.Empty<Genre>().AsQueryable();

        _mockUnitOfWork.Setup(x => x.GenreRepository.GetAll())
            .Returns(genres);

        var genreService = new GenreService(_mockUnitOfWork.Object, _mapper);

        // act
        var result = genreService.GetAll();

        // assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetAll_ThereAreGenres_ReturnNotEmptyList()
    {
        // arrange
        var genres = TestHelper.GetGenres(3);
        var genreDtos = TestHelper.GetGenresDtos(genres);

        _mockUnitOfWork.Setup(x => x.GenreRepository.GetAll())
            .Returns(genres);

        var genreService = new GenreService(_mockUnitOfWork.Object, _mapper);

        // act
        var result = genreService.GetAll();

        // assert
        Assert.Equal(genreDtos.Count(), result.Count());
    }

    [Fact]
    public void GetAllByParentId_ThereAreNoGenres_ReturnEmptyList()
    {
        // arrange
        Guid parentGenreId = Guid.NewGuid();
        var genres = new List<Genre>()
        {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
        };

        _mockUnitOfWork.Setup(x => x.GenreRepository.GetAll())
            .Returns(genres.AsQueryable());

        var genreService = new GenreService(_mockUnitOfWork.Object, _mapper);

        // act
        var result = genreService.GetAllByParentId(parentGenreId);

        // assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetAllByParentId_ThereAreGenres_ReturnNotEmptyList()
    {
        // arrange
        Guid parentGenreId = Guid.NewGuid();
        var genres = new List<Genre>()
        {
            new() { Id = Guid.NewGuid(), ParentGenreId = parentGenreId },
            new() { Id = Guid.NewGuid(), ParentGenreId = parentGenreId },
        };

        _mockUnitOfWork.Setup(x => x.GenreRepository.GetAll())
            .Returns(genres.AsQueryable());

        var genreService = new GenreService(_mockUnitOfWork.Object, _mapper);

        // act
        var result = genreService.GetAllByParentId(parentGenreId);

        // assert
        Assert.Equal(genres.Count, result.Count());
    }

    [Fact]
    public async Task GetAllByGameKey_GameExistsGenresExist_ReturnNotEmptyList()
    {
        // arrange
        string existedGameKey = "testGame";
        var games = new List<Game>()
        {
            new() { Id = Guid.NewGuid(), Key = existedGameKey },
        };
        var genres = new List<Genre>()
        {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
        };
        var gameGenres = new List<GameGenre>()
        {
            new() { GameId = games[0].Id, GenreId = genres[0].Id },
            new() { GameId = games[0].Id, GenreId = genres[1].Id },
        };

        _mockUnitOfWork.Setup(x => x.GenreRepository.GetAll())
            .Returns(genres.AsQueryable());
        _mockUnitOfWork.Setup(x => x.GameRepository.GetByKeyAsync(existedGameKey))
            .ReturnsAsync(games[0]);
        _mockUnitOfWork.Setup(x => x.GameRepository.GetAll())
            .Returns(games.AsQueryable());
        _mockUnitOfWork.Setup(x => x.GameGenreRepository.GetAll())
            .Returns(gameGenres.AsQueryable());

        var genreService = new GenreService(_mockUnitOfWork.Object, _mapper);

        // act
        var result = await genreService.GetAllByGameKey(existedGameKey);

        // assert
        Assert.Equal(genres.Count, result.Count());
    }

    [Fact]
    public async Task GetAllByGameKey_GameExistsGenresDoNotExist_ReturnEmptyList()
    {
        // arrange
        string existedGameKey = "testGame";
        var games = new List<Game>()
        {
            new() { Id = Guid.NewGuid(), Key = existedGameKey },
        };
        var genres = new List<Genre>();
        var gameGenres = new List<GameGenre>();

        _mockUnitOfWork.Setup(x => x.GenreRepository.GetAll())
            .Returns(genres.AsQueryable());
        _mockUnitOfWork.Setup(x => x.GameRepository.GetByKeyAsync(existedGameKey))
            .ReturnsAsync(games[0]);
        _mockUnitOfWork.Setup(x => x.GameRepository.GetAll())
            .Returns(games.AsQueryable());
        _mockUnitOfWork.Setup(x => x.GameGenreRepository.GetAll())
            .Returns(gameGenres.AsQueryable());

        var genreService = new GenreService(_mockUnitOfWork.Object, _mapper);

        // act
        var result = await genreService.GetAllByGameKey(existedGameKey);

        // assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllByGameKey_GameDoesNotExist_ThrowNotFoundException()
    {
        // arrange
        string notExistedGameKey = " ";

        _mockUnitOfWork.Setup(x => x.GameRepository.GetByKeyAsync(notExistedGameKey))
            .ReturnsAsync((Game)null!);

        var genreService = new GenreService(_mockUnitOfWork.Object, _mapper);

        // act & assert
        await Assert.ThrowsAsync<NotFoundException>(() => genreService.GetAllByGameKey(notExistedGameKey));
    }

    #endregion

    #region Delete tests

    [Fact]
    public async Task Delete_GenreExists_DeleteSuccessfully()
    {
        // arrange
        var existedGenreId = Guid.NewGuid();
        var genre = new Genre() { Id = existedGenreId };

        _mockUnitOfWork.Setup(x => x.GenreRepository.GetByIdAsync(existedGenreId))
            .ReturnsAsync(genre);

        var genreService = new GenreService(_mockUnitOfWork.Object, _mapper);

        // act
        await genreService.DeleteAsync(existedGenreId);

        // assert
        _mockUnitOfWork.Verify(x => x.GenreRepository.Delete(genre), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Delete_GenreDoesNotExist_DoesNotDelete()
    {
        // arrange
        var notExistedGenreId = Guid.NewGuid();

        _mockUnitOfWork.Setup(x => x.GenreRepository.GetByIdAsync(notExistedGenreId))
            .ReturnsAsync((Genre)null);

        var genreService = new GenreService(_mockUnitOfWork.Object, _mapper);

        // act
        await genreService.DeleteAsync(notExistedGenreId);

        // assert
        _mockUnitOfWork.Verify(x => x.GenreRepository.Delete(It.IsAny<Genre>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    #endregion

    #region Add tests

    [Fact]
    public async Task Add_GenreNameIsValid_AddGenre()
    {
        // arrange
        var validGenreCreateRequest = new GenreCreateRequest
        {
            Genre = new GenreCreateRequestInfo()
            {
                Name = "Test Genre",
            },
        };

        _mockUnitOfWork.Setup(x => x.GenreRepository.AddAsync(It.IsAny<Genre>()))
            .ReturnsAsync((Genre g) => g);

        var genreService = new GenreService(_mockUnitOfWork.Object, _mapper);

        // act
        var result = await genreService.AddAsync(validGenreCreateRequest);

        // assert
        Assert.NotNull(result);
        Assert.Equal(validGenreCreateRequest.Genre.Name, result.Name);
        _mockUnitOfWork.Verify(x => x.GenreRepository.AddAsync(It.IsAny<Genre>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Add_GenreNameIsNull_ThrowBadRequestException()
    {
        // arrange
        var invalidGenreCreateRequest = new GenreCreateRequest
        {
            Genre = new GenreCreateRequestInfo(), // name is absent
        };

        _mockUnitOfWork.Setup(x => x.GenreRepository.AddAsync(It.IsAny<Genre>()))
            .ReturnsAsync((Genre g) => g);

        var genreService = new GenreService(_mockUnitOfWork.Object, _mapper);

        // act & assert
        await Assert.ThrowsAsync<BadRequestException>(() => genreService.AddAsync(invalidGenreCreateRequest));
    }

    #endregion

    #region Update tests

    [Fact]
    public async Task Update_GenreExists_UpdateGenre()
    {
        // arrange
        var originalGenre = new Genre()
        {
            Id = Guid.NewGuid(),
            Name = "Test Genre",
        };
        var genreUpdateRequest = new GenreUpdateRequest
        {
            Genre = new GenreDto()
            {
                Id = originalGenre.Id,
                Name = "New genre name",
            },
        };

        _mockUnitOfWork.Setup(x => x.GenreRepository.GetByIdAsync(genreUpdateRequest.Genre.Id))
            .ReturnsAsync(originalGenre);

        var mockMapper = new Mock<IMapper>();
        var gameService = new GenreService(_mockUnitOfWork.Object, mockMapper.Object);

        // act
        await gameService.UpdateAsync(genreUpdateRequest);

        // assert
        mockMapper.Verify(m => m.Map(genreUpdateRequest.Genre, originalGenre), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Update_GenreDoesNotExist_ThrowNotFoundException()
    {
        // arrange
        var notExistedId = Guid.NewGuid();
        var gameUpdateRequest = new GenreUpdateRequest
        {
            Genre = new GenreDto() { Id = notExistedId, Name = "Some name" },
        };

        _mockUnitOfWork.Setup(x => x.GenreRepository.GetByIdAsync(notExistedId))
            .ReturnsAsync((Genre)null);

        var gameService = new GenreService(_mockUnitOfWork.Object, _mapper);

        // act & assert
        await Assert.ThrowsAsync<NotFoundException>(() => gameService.UpdateAsync(gameUpdateRequest));
    }

    [Fact]
    public async Task Update_GenreNameIsNull_ThrowBadRequestException()
    {
        // arrange
        var gameUpdateRequest = new GenreUpdateRequest
        {
            Genre = new GenreDto() { Id = Guid.NewGuid() }, // name is absent
        };

        var gameService = new GenreService(_mockUnitOfWork.Object, _mapper);

        // act & assert
        await Assert.ThrowsAsync<BadRequestException>(() => gameService.UpdateAsync(gameUpdateRequest));
    }

    #endregion
}
