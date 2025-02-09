using AutoMapper;
using BusinessLogic.Dto.Platform;
using BusinessLogic.Exceptions;
using BusinessLogic.Mappings;
using BusinessLogic.Requests.Platform;
using BusinessLogic.Services;
using DataAccess.Entities;
using DataAccess.UnitOfWork;
using Moq;
using XUnitTests.Helpers;

namespace XUnitTests;

public class PlatformServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;

    private readonly IMapper _mapper;

    public PlatformServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        var config = new MapperConfiguration(cfg => { cfg.AddProfile<PlatformProfile>(); });
        _mapper = config.CreateMapper();
    }

    #region Get tests

    [Fact]
    public async Task GetById_PlatformDoesNotExist_ThrowNotFoundException()
    {
        // arrange
        Guid notExistedGameId = Guid.NewGuid();

        _mockUnitOfWork.Setup(x => x.PlatformRepository.GetByIdAsync(notExistedGameId))
            .ReturnsAsync((Platform)null);

        var platformService = new PlatformService(_mockUnitOfWork.Object, _mapper);

        // act & assert
        await Assert.ThrowsAsync<NotFoundException>(() => platformService.GetByIdAsync(notExistedGameId));
    }

    [Fact]
    public async Task GetById_PlatformExists_ReturnGameDto()
    {
        // arrange
        Guid existedPlatformId = Guid.NewGuid();
        var platform = new Platform() { Id = existedPlatformId };

        _mockUnitOfWork.Setup(x => x.PlatformRepository.GetByIdAsync(existedPlatformId))
            .ReturnsAsync(platform);

        var platformService = new PlatformService(_mockUnitOfWork.Object, _mapper);

        // act
        var result = await platformService.GetByIdAsync(existedPlatformId);

        // assert
        Assert.NotNull(result);
        Assert.Equal(existedPlatformId, result.Id);
        Assert.IsType<PlatformDto>(result);
    }

    [Fact]
    public void GetAll_ThereAreNoPlatforms_ReturnEmptyList()
    {
        // arrange
        var platforms = Enumerable.Empty<Platform>().AsQueryable();

        _mockUnitOfWork.Setup(x => x.PlatformRepository.GetAll())
            .Returns(platforms);

        var platformService = new PlatformService(_mockUnitOfWork.Object, _mapper);

        // act
        var result = platformService.GetAll();

        // assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetAll_ThereArePlatforms_ReturnNotEmptyList()
    {
        // arrange
        var platforms = TestHelper.GetPlatforms(3);
        var platformDtos = TestHelper.GetPlatformsDtos(platforms);

        _mockUnitOfWork.Setup(x => x.PlatformRepository.GetAll())
            .Returns(platforms);

        var platformService = new PlatformService(_mockUnitOfWork.Object, _mapper);

        // act
        var result = platformService.GetAll();

        // assert
        Assert.Equal(platformDtos.Count(), result.Count());
    }

    [Fact]
    public async Task GetAllByGameKey_GameExistsPlatformsExist_ReturnNotEmptyList()
    {
        // arrange
        string existedGameKey = "testGame";
        var games = new List<Game>()
        {
            new() { Id = Guid.NewGuid(), Key = existedGameKey },
        };
        var platforms = new List<Platform>()
        {
            new() { Id = Guid.NewGuid() },
            new() { Id = Guid.NewGuid() },
        };
        var gamePlatforms = new List<GamePlatform>()
        {
            new() { GameId = games[0].Id, PlatformId = platforms[0].Id },
            new() { GameId = games[0].Id, PlatformId = platforms[1].Id },
        };

        _mockUnitOfWork.Setup(x => x.PlatformRepository.GetAll())
            .Returns(platforms.AsQueryable());
        _mockUnitOfWork.Setup(x => x.GameRepository.GetByKeyAsync(existedGameKey))
            .ReturnsAsync(games[0]);
        _mockUnitOfWork.Setup(x => x.GameRepository.GetAll())
            .Returns(games.AsQueryable());
        _mockUnitOfWork.Setup(x => x.GamePlatformRepository.GetAll())
            .Returns(gamePlatforms.AsQueryable());

        var platformService = new PlatformService(_mockUnitOfWork.Object, _mapper);

        // act
        var result = await platformService.GetAllByGameKey(existedGameKey);

        // assert
        Assert.Equal(platforms.Count, result.Count());
    }

    [Fact]
    public async Task GetAllByGameKey_GameExistsPlatformsDoNotExist_ReturnEmptyList()
    {
        // arrange
        string existedGameKey = "testGame";
        var games = new List<Game>()
        {
            new() { Id = Guid.NewGuid(), Key = existedGameKey },
        };
        var platforms = new List<Platform>();
        var gamePlatforms = new List<GamePlatform>();

        _mockUnitOfWork.Setup(x => x.PlatformRepository.GetAll())
            .Returns(platforms.AsQueryable());
        _mockUnitOfWork.Setup(x => x.GameRepository.GetByKeyAsync(existedGameKey))
            .ReturnsAsync(games[0]);
        _mockUnitOfWork.Setup(x => x.GameRepository.GetAll())
            .Returns(games.AsQueryable());
        _mockUnitOfWork.Setup(x => x.GamePlatformRepository.GetAll())
            .Returns(gamePlatforms.AsQueryable());

        var platformService = new PlatformService(_mockUnitOfWork.Object, _mapper);

        // act
        var result = await platformService.GetAllByGameKey(existedGameKey);

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

        var platformService = new PlatformService(_mockUnitOfWork.Object, _mapper);

        // act & assert
        await Assert.ThrowsAsync<NotFoundException>(() => platformService.GetAllByGameKey(notExistedGameKey));
    }

    #endregion

    #region Delete tests

    [Fact]
    public async Task Delete_PlatformExists_DeleteSuccessfully()
    {
        // arrange
        var existedPlatformId = Guid.NewGuid();
        var platform = new Platform() { Id = existedPlatformId };

        _mockUnitOfWork.Setup(x => x.PlatformRepository.GetByIdAsync(existedPlatformId))
            .ReturnsAsync(platform);

        var platformService = new PlatformService(_mockUnitOfWork.Object, _mapper);

        // act
        await platformService.DeleteAsync(existedPlatformId);

        // assert
        _mockUnitOfWork.Verify(x => x.PlatformRepository.Delete(platform), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Delete_PlatformDoesNotExist_DoesNotDelete()
    {
        // arrange
        var notExistedPlatformId = Guid.NewGuid();

        _mockUnitOfWork.Setup(x => x.PlatformRepository.GetByIdAsync(notExistedPlatformId))
            .ReturnsAsync((Platform)null);

        var platformService = new PlatformService(_mockUnitOfWork.Object, _mapper);

        // act
        await platformService.DeleteAsync(notExistedPlatformId);

        // assert
        _mockUnitOfWork.Verify(x => x.PlatformRepository.Delete(It.IsAny<Platform>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    #endregion

    #region Add tests

    [Fact]
    public async Task Add_PlatformTypeIsValid_AddPlatform()
    {
        // arrange
        var validPlatformCreateRequest = new PlatformCreateRequest
        {
            Platform = new PlatformCreateRequestInfo()
            {
                Type = "Test type",
            },
        };

        _mockUnitOfWork.Setup(x => x.PlatformRepository.AddAsync(It.IsAny<Platform>()))
            .ReturnsAsync((Platform g) => g);

        var platformService = new PlatformService(_mockUnitOfWork.Object, _mapper);

        // act
        var result = await platformService.AddAsync(validPlatformCreateRequest);

        // assert
        Assert.NotNull(result);
        Assert.Equal(validPlatformCreateRequest.Platform.Type, result.Type);
        _mockUnitOfWork.Verify(x => x.PlatformRepository.AddAsync(It.IsAny<Platform>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Add_PlatformTypeIsNull_ThrowBadRequestException()
    {
        // arrange
        var invalidPlatformCreateRequest = new PlatformCreateRequest
        {
            Platform = new PlatformCreateRequestInfo(), // type is absent
        };

        _mockUnitOfWork.Setup(x => x.PlatformRepository.AddAsync(It.IsAny<Platform>()))
            .ReturnsAsync((Platform g) => g);

        var platformService = new PlatformService(_mockUnitOfWork.Object, _mapper);

        // act & assert
        await Assert.ThrowsAsync<BadRequestException>(() => platformService.AddAsync(invalidPlatformCreateRequest));
    }

    #endregion

    #region Update tests

    [Fact]
    public async Task Update_PlatformExists_UpdatePlatform()
    {
        // arrange
        var originalPlatform = new Platform()
        {
            Id = Guid.NewGuid(),
            Type = "Test type",
        };
        var platformUpdateRequest = new PlatformUpdateRequest
        {
            Platform = new PlatformDto()
            {
                Id = originalPlatform.Id,
                Type = "New platform type",
            },
        };

        _mockUnitOfWork.Setup(x => x.PlatformRepository.GetByIdAsync(platformUpdateRequest.Platform.Id))
            .ReturnsAsync(originalPlatform);

        var mockMapper = new Mock<IMapper>();
        var gameService = new PlatformService(_mockUnitOfWork.Object, mockMapper.Object);

        // act
        await gameService.UpdateAsync(platformUpdateRequest);

        // assert
        mockMapper.Verify(m => m.Map(platformUpdateRequest.Platform, originalPlatform), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Update_PlatformDoesNotExist_ThrowNotFoundException()
    {
        // arrange
        var notExistedId = Guid.NewGuid();
        var gameUpdateRequest = new PlatformUpdateRequest
        {
            Platform = new PlatformDto() { Id = notExistedId, Type = "Some type" },
        };

        _mockUnitOfWork.Setup(x => x.PlatformRepository.GetByIdAsync(notExistedId))
            .ReturnsAsync((Platform)null);

        var gameService = new PlatformService(_mockUnitOfWork.Object, _mapper);

        // act & assert
        await Assert.ThrowsAsync<NotFoundException>(() => gameService.UpdateAsync(gameUpdateRequest));
    }

    [Fact]
    public async Task Update_PlatformNameIsNull_ThrowBadRequestException()
    {
        // arrange
        var gameUpdateRequest = new PlatformUpdateRequest
        {
            Platform = new PlatformDto() { Id = Guid.NewGuid() }, // name is absent
        };

        var gameService = new PlatformService(_mockUnitOfWork.Object, _mapper);

        // act & assert
        await Assert.ThrowsAsync<BadRequestException>(() => gameService.UpdateAsync(gameUpdateRequest));
    }

    #endregion
}
