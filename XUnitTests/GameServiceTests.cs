using System.Text;
using System.Text.Json;
using AutoMapper;
using BusinessLogic.Dto.Game;
using BusinessLogic.Exceptions;
using BusinessLogic.Mappings;
using BusinessLogic.Requests.Game;
using BusinessLogic.Services;
using DataAccess.Entities;
using DataAccess.UnitOfWork;
using Moq;
using XUnitTests.Helpers;

namespace XUnitTests;

public class GameServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;

    private readonly IMapper _mapper;

    public GameServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        var config = new MapperConfiguration(cfg => { cfg.AddProfile<GameProfile>(); });
        _mapper = config.CreateMapper();
    }

    #region Get tests

    [Fact]
    public async Task GetById_GameDoesNotExist_ThrowNotFoundException()
    {
        // arrange
        Guid notExistedGameId = Guid.NewGuid();

        _mockUnitOfWork.Setup(x => x.GameRepository.GetByIdAsync(notExistedGameId))
            .ReturnsAsync((Game)null);

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act & assert
        await Assert.ThrowsAsync<NotFoundException>(() => gameService.GetByIdAsync(notExistedGameId));
    }

    [Fact]
    public async Task GetById_GameExists_ReturnGameDto()
    {
        // arrange
        Guid existedGameId = Guid.NewGuid();
        var game = new Game() { Id = existedGameId };

        _mockUnitOfWork.Setup(x => x.GameRepository.GetByIdAsync(existedGameId))
            .ReturnsAsync(game);

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act
        var gameResult = await gameService.GetByIdAsync(existedGameId);

        // assert
        Assert.NotNull(gameResult);
        Assert.Equal(existedGameId, gameResult.Id);
        Assert.IsType<GameDto>(gameResult);
    }

    [Fact]
    public async Task GetByKey_GameDoesNotExist_ThrowNotFoundException()
    {
        // arrange
        string notExistedGameKey = " ";

        _mockUnitOfWork.Setup(x => x.GameRepository.GetByKeyAsync(notExistedGameKey))
            .ReturnsAsync((Game)null!);

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act & assert
        await Assert.ThrowsAsync<NotFoundException>(() => gameService.GetByKeyAsync(notExistedGameKey));
    }

    [Fact]
    public async Task GetByKey_GameExist_ReturnGameDto()
    {
        // arrange
        string existedGameKey = "gamekey";
        var game = new Game() { Key = existedGameKey };

        _mockUnitOfWork.Setup(x => x.GameRepository.GetByKeyAsync(existedGameKey))
            .ReturnsAsync(game);

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act
        var gameResult = await gameService.GetByKeyAsync(existedGameKey);

        // assert
        Assert.NotNull(gameResult);
        Assert.Equal(existedGameKey, gameResult.Key);
        Assert.IsType<GameDto>(gameResult);
    }

    [Fact]
    public void GetAll_ThereAreNoGames_ReturnEmptyList()
    {
        // arrange
        var games = Enumerable.Empty<Game>().AsQueryable();

        _mockUnitOfWork.Setup(x => x.GameRepository.GetAll())
            .Returns(games);

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act
        var gameResult = gameService.GetAll();

        // assert
        Assert.Empty(gameResult);
    }

    [Fact]
    public void GetAll_ThereAreGames_ReturnNotEmptyList()
    {
        // arrange
        var games = TestHelper.GetGames(3);
        var gameDtos = TestHelper.GetGamesDtos(games);

        _mockUnitOfWork.Setup(x => x.GameRepository.GetAll())
            .Returns(games);

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act
        var gameResult = gameService.GetAll();

        // assert
        Assert.Equal(gameDtos.Count(), gameResult.Count());
    }

    [Fact]
    public void GetAllByGenreId_ThereAreNoGames_ReturnEmptyList()
    {
        // arrange
        var genreId = Guid.NewGuid();
        var games = Enumerable.Empty<Game>().AsQueryable();
        var genres = Enumerable.Empty<Genre>().AsQueryable();
        var gameGenres = Enumerable.Empty<GameGenre>().AsQueryable();

        SetupRepositoriesGetAllByGenreId(games, genres, gameGenres);

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act
        var gameResult = gameService.GetAllByGenreId(genreId);

        // assert
        Assert.Empty(gameResult);
    }

    [Fact]
    public void GetAllByGenreId_ThereAreGames_ReturnNotEmptyList()
    {
        // arrange
        var games = TestHelper.GetGames(3);
        var genres = TestHelper.GetGenres(2);

        // link games to first genre
        var genreId = genres.ToArray()[0].Id;
        var gameGenres = games.Select(g => new GameGenre() { GameId = g.Id, GenreId = genreId });

        SetupRepositoriesGetAllByGenreId(games, genres, gameGenres);

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act
        var gameResult = gameService.GetAllByGenreId(genreId);

        // assert
        Assert.Equal(gameGenres.Count(), gameResult.Count());
    }

    [Fact]
    public async Task GetGameFile_GameExist_ReturnGameBytes()
    {
        // arrange
        string existedGameKey = "gamekey";
        var game = new Game() { Key = existedGameKey };
        var json = JsonSerializer.Serialize(game);
        var expectedBytes = Encoding.UTF8.GetBytes(json);

        _mockUnitOfWork.Setup(x => x.GameRepository.GetByKeyAsync(existedGameKey))
            .ReturnsAsync(game);

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act
        var actualBytes = await gameService.GetGameFileAsync(existedGameKey);

        // assert
        Assert.Equal(expectedBytes, actualBytes);
    }

    [Fact]
    public async Task GetGameFile_GameDoesNotExist_ThrowNotFoundException()
    {
        // arrange
        string notExistedGameKey = " ";

        _mockUnitOfWork.Setup(x => x.GameRepository.GetByKeyAsync(notExistedGameKey))
            .ReturnsAsync((Game)null!);

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act & assert
        await Assert.ThrowsAsync<NotFoundException>(() => gameService.GetGameFileAsync(notExistedGameKey));
    }

    [Fact]
    public void GetTotalGamesCount_GamesExist_ReturnTotalGamesCount()
    {
        // arrange
        var games = TestHelper.GetGames(3);

        _mockUnitOfWork.Setup(x => x.GameRepository.GetAll())
            .Returns(games);

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act
        var result = gameService.GetAll();

        // assert
        Assert.Equal(games.Count(), result.Count());
    }

    [Fact]
    public void GetTotalGamesCount_GamesDoNotExist_ReturnZero()
    {
        // arrange
        var games = Enumerable.Empty<Game>().AsQueryable();

        _mockUnitOfWork.Setup(x => x.GameRepository.GetAll())
            .Returns(games);

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act
        var result = gameService.GetAll();

        // assert
        Assert.Equal(0, result.Count());
    }

    #endregion

    #region Delete tests

    [Fact]
    public async Task DeleteById_GameExists_DeleteSuccessfully()
    {
        // arrange
        var existedGameId = Guid.NewGuid();
        var game = new Game() { Id = existedGameId };

        _mockUnitOfWork.Setup(x => x.GameRepository.GetByIdAsync(existedGameId))
            .ReturnsAsync(game);
        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act
        await gameService.DeleteByIdAsync(existedGameId);

        // assert
        _mockUnitOfWork.Verify(x => x.GameRepository.Delete(game), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteById_GameDoesNotExist_DoesNotDelete()
    {
        // arrange
        var notExistedGameId = Guid.NewGuid();

        _mockUnitOfWork.Setup(x => x.GameRepository.GetByIdAsync(notExistedGameId))
            .ReturnsAsync((Game)null);

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act
        await gameService.DeleteByIdAsync(notExistedGameId);

        // assert
        _mockUnitOfWork.Verify(x => x.GameRepository.Delete(It.IsAny<Game>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteByKey_GameExists_DeleteSuccessfully()
    {
        // arrange
        var existedGameKey = "gamekey";
        var game = new Game() { Key = existedGameKey };

        _mockUnitOfWork.Setup(x => x.GameRepository.GetByKeyAsync(existedGameKey))
            .ReturnsAsync(game);

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act
        await gameService.DeleteByKeyAsync(existedGameKey);

        // assert
        _mockUnitOfWork.Verify(x => x.GameRepository.Delete(game), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteByKey_GameDoesNotExist_DoesNotDelete()
    {
        // arrange
        var notExistedGameKey = " ";

        _mockUnitOfWork.Setup(x => x.GameRepository.GetByKeyAsync(notExistedGameKey))
            .ReturnsAsync((Game)null!);

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act
        await gameService.DeleteByKeyAsync(notExistedGameKey);

        // assert
        _mockUnitOfWork.Verify(x => x.GameRepository.Delete(It.IsAny<Game>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    #endregion

    #region Add tests

    [Fact]
    public async Task Add_GameOnlyWithNameAndKey_AddGame()
    {
        // arrange
        var validGameCreateRequest = new GameCreateRequest
        {
            Game = new GameCreateRequestInfo()
            {
                Name = "The Witcher",
                Key = "theWitcher",
            },
        };

        _mockUnitOfWork.Setup(x => x.GameRepository.AddAsync(It.IsAny<Game>()))
            .ReturnsAsync((Game g) => g);

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act
        var result = await gameService.AddAsync(validGameCreateRequest);

        // assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(validGameCreateRequest.Game.Name, result.Name);
        Assert.Equal(validGameCreateRequest.Game.Key, result.Key);
    }

    [Fact]
    public async Task Add_GameOnlyNameIsNull_ThrowBadRequestException()
    {
        // arrange
        var invalidGameCreateRequest = new GameCreateRequest
        {
            Game = new GameCreateRequestInfo(), // name is absent
        };

        _mockUnitOfWork.Setup(x => x.GameRepository.AddAsync(It.IsAny<Game>()))
            .ReturnsAsync((Game g) => g);

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act & assert
        await Assert.ThrowsAsync<BadRequestException>(() => gameService.AddAsync(invalidGameCreateRequest));
    }

    [Theory]
    [InlineData("The Witcher", "theWitcher")]
    [InlineData("Elder Scrolls V: Skyrim", "elderScrollsVSkyrim")]
    [InlineData("Cyberpunk 2077", "cyberpunk2077")]
    [InlineData("Grand Theft Auto V", "grandTheftAutoV")]
    [InlineData("Super_Mario_Bros", "superMarioBros")]
    [InlineData("Call of Duty: Modern Warfare", "callOfDutyModernWarfare")]
    [InlineData("Half-Life 2", "halfLife2")]
    public async Task Add_GameOnlyKeyIsNull_AddGameWithGeneratedKey(string gameName, string expectedKey)
    {
        // arrange
        var gameCreateRequest = new GameCreateRequest
        {
            Game = new GameCreateRequestInfo()
            {
                Name = gameName,
            },
        };

        _mockUnitOfWork.Setup(x => x.GameRepository.AddAsync(It.IsAny<Game>()))
            .ReturnsAsync((Game g) => g);

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act
        var result = await gameService.AddAsync(gameCreateRequest);

        // assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(expectedKey, result.Key);
        Assert.Equal(gameCreateRequest.Game.Name, result.Name);
    }

    [Fact]
    public async Task Add_GameWithGenresAndPlatforms_AddGameAndLinkingEntities()
    {
        // Arrange
        var genres = TestHelper.GetGenres(2);
        var platforms = TestHelper.GetPlatforms(1);
        var gameCreateRequest = new GameCreateRequest
        {
            Game = new GameCreateRequestInfo()
            {
                Name = "The Witcher",
                Key = "theWitcher",
            },
            Genres = genres.Select(g => g.Id),
            Platforms = platforms.Select(p => p.Id),
        };

        _mockUnitOfWork.Setup(x => x.GameRepository.AddAsync(It.IsAny<Game>()))
            .ReturnsAsync((Game g) => g);
        _mockUnitOfWork.Setup(x => x.GameGenreRepository.AddAsync(It.IsAny<GameGenre>()));
        _mockUnitOfWork.Setup(x => x.GamePlatformRepository.AddAsync(It.IsAny<GamePlatform>()));

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // Act
        var result = await gameService.AddAsync(gameCreateRequest);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(gameCreateRequest.Game.Name, result.Name);
        Assert.Equal(gameCreateRequest.Game.Key, result.Key);

        _mockUnitOfWork.Verify(x => x.GameRepository.AddAsync(It.IsAny<Game>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.GameGenreRepository.AddAsync(It.IsAny<GameGenre>()), Times.Exactly(genres.Count()));
        _mockUnitOfWork.Verify(x => x.GamePlatformRepository.AddAsync(It.IsAny<GamePlatform>()), Times.Exactly(platforms.Count()));
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    #endregion

    #region Update tests

    [Fact]
    public async Task Update_GameExists_UpdateGame()
    {
        // arrange
        var originalGame = new Game()
        {
            Id = Guid.NewGuid(),
            Name = "The Witcher",
            Key = "theWitcher",
        };
        var gameUpdateRequest = new GameUpdateRequest
        {
            Game = new GameDto()
            {
                Id = originalGame.Id,
                Name = "New name",
                Key = "newName",
            },
        };

        _mockUnitOfWork.Setup(x => x.GameRepository.GetByIdAsync(gameUpdateRequest.Game.Id))
            .ReturnsAsync(originalGame);
        _mockUnitOfWork.Setup(x => x.GameGenreRepository.AddAsync(It.IsAny<GameGenre>()));
        _mockUnitOfWork.Setup(x => x.GamePlatformRepository.AddAsync(It.IsAny<GamePlatform>()));

        var mockMapper = new Mock<IMapper>();
        var gameService = new GameService(_mockUnitOfWork.Object, mockMapper.Object);

        // act
        await gameService.UpdateAsync(gameUpdateRequest);

        // assert
        mockMapper.Verify(m => m.Map(gameUpdateRequest.Game, originalGame), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Update_GameDoesNotExist_ThrowNotFoundException()
    {
        // arrange
        var notExistedId = Guid.NewGuid();
        var gameUpdateRequest = new GameUpdateRequest
        {
            Game = new GameDto()
            {
                Id = notExistedId,
                Name = "Test Game",
            },
        };

        _mockUnitOfWork.Setup(x => x.GameRepository.GetByIdAsync(notExistedId))
            .ReturnsAsync((Game)null);

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act & assert
        await Assert.ThrowsAsync<NotFoundException>(() => gameService.UpdateAsync(gameUpdateRequest));
    }

    [Fact]
    public async Task Update_GameOnlyNameIsNull_ThrowBadRequestException()
    {
        // arrange
        var notNullGame = new Game { Id = Guid.NewGuid() };
        var invalidGameUpdateRequest = new GameUpdateRequest
        {
            Game = new GameDto() // name is absent
            {
                Id = notNullGame.Id,
            },
        };

        _mockUnitOfWork.Setup(x => x.GameRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(notNullGame);

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act & assert
        await Assert.ThrowsAsync<BadRequestException>(() => gameService.UpdateAsync(invalidGameUpdateRequest));
    }

    [Theory]
    [InlineData("The Witcher", "theWitcher")]
    [InlineData("Elder Scrolls V: Skyrim", "elderScrollsVSkyrim")]
    [InlineData("Cyberpunk 2077", "cyberpunk2077")]
    [InlineData("Grand Theft Auto V", "grandTheftAutoV")]
    [InlineData("Super_Mario_Bros", "superMarioBros")]
    [InlineData("Call of Duty: Modern Warfare", "callOfDutyModernWarfare")]
    [InlineData("Half-Life 2", "halfLife2")]
    public async Task Update_GameOnlyKeyIsNull_UpdateGameWithGeneratedKey(string gameName, string expectedKey)
    {
        // arrange
        var originalGame = new Game { Id = Guid.NewGuid(), Name = gameName };
        var gameUpdateRequest = new GameUpdateRequest
        {
            Game = new GameDto() // key is absent
            {
                Id = originalGame.Id,
                Name = originalGame.Name,
            },
        };

        _mockUnitOfWork.Setup(x => x.GameRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(originalGame);
        _mockUnitOfWork.Setup(x => x.GameGenreRepository.GetAll());
        _mockUnitOfWork.Setup(x => x.GamePlatformRepository.GetAll());

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // act
        await gameService.UpdateAsync(gameUpdateRequest);

        // assert
        Assert.Equal(expectedKey, originalGame.Key);
    }

    [Fact]
    public async Task Update_GameWithGenresAndPlatforms_UpdateGameAndLinkingEntities()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var originalGame = new Game { Id = gameId, Name = "Test Game" };
        var request = new GameUpdateRequest
        {
            Game = new GameDto { Id = gameId, Name = "Test Game" },
            Genres = [Guid.NewGuid(), Guid.NewGuid()],
            Platforms = [Guid.NewGuid()],
        };

        var oldGenres = new List<GameGenre>
        {
            new() { GameId = gameId, GenreId = Guid.NewGuid() },
        };
        var oldPlatforms = new List<GamePlatform>
        {
            new() { GameId = gameId, PlatformId = Guid.NewGuid() },
        };

        _mockUnitOfWork.Setup(x => x.GameRepository.GetByIdAsync(gameId))
            .ReturnsAsync(originalGame);
        _mockUnitOfWork.Setup(u => u.GameGenreRepository.GetAll())
            .Returns(oldGenres.AsQueryable());
        _mockUnitOfWork.Setup(u => u.GamePlatformRepository.GetAll())
            .Returns(oldPlatforms.AsQueryable());

        var gameService = new GameService(_mockUnitOfWork.Object, _mapper);

        // Act
        await gameService.UpdateAsync(request);

        // Assert
        _mockUnitOfWork.Verify(u => u.GameGenreRepository.DeleteRange(oldGenres), Times.Once);
        _mockUnitOfWork.Verify(u => u.GamePlatformRepository.DeleteRange(oldPlatforms), Times.Once);
        _mockUnitOfWork.Verify(u => u.GameGenreRepository.AddAsync(It.IsAny<GameGenre>()), Times.Exactly(2));
        _mockUnitOfWork.Verify(u => u.GamePlatformRepository.AddAsync(It.IsAny<GamePlatform>()), Times.Once);
    }

    #endregion

    #region Setupers

    private void SetupRepositoriesGetAllByGenreId(
        IQueryable<Game> games,
        IQueryable<Genre> genres,
        IQueryable<GameGenre> gameGenres)
    {
        _mockUnitOfWork.Setup(x => x.GameRepository.GetAll())
            .Returns(games);
        _mockUnitOfWork.Setup(x => x.GenreRepository.GetAll())
            .Returns(genres);
        _mockUnitOfWork
            .Setup(x => x.GameGenreRepository.GetAll())
            .Returns(gameGenres);
    }

    #endregion
}
