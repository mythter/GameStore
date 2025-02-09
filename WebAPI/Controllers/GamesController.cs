using BusinessLogic.Requests.Game;
using BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers;

[Route("/api")]
[ApiController]
public class GamesController(IGameService gameService) : ControllerBase
{
    [HttpGet("[controller]/find/{id}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        var game = await gameService.GetByIdAsync(id);
        return Ok(game);
    }

    [HttpGet("[controller]/{key}")]
    public async Task<ActionResult> GetByKey(string key)
    {
        var game = await gameService.GetByKeyAsync(key);
        return Ok(game);
    }

    [HttpGet("[controller]")]
    public async Task<ActionResult> GetAll()
    {
        var games = await gameService.GetAll().ToListAsync();
        return Ok(games);
    }

    [HttpGet("genres/{id}/games")]
    public async Task<ActionResult> GetGamesByGenreId(Guid id)
    {
        var genres = await gameService.GetAllByGenreId(id).ToListAsync();
        return Ok(genres);
    }

    [HttpPost("[controller]")]
    public async Task<ActionResult> Create([FromBody] GameCreateRequest request)
    {
        var gameDto = await gameService.AddAsync(request);
        return Ok(gameDto);
    }

    [HttpPut("[controller]")]
    public async Task<ActionResult> Update([FromBody] GameUpdateRequest game)
    {
        await gameService.UpdateAsync(game);
        return Ok();
    }

    [HttpDelete("[controller]/delete/{id}")]
    public async Task<ActionResult> DeleteById(Guid id)
    {
        await gameService.DeleteByIdAsync(id);
        return Ok();
    }

    [HttpDelete("[controller]/{key}")]
    public async Task<ActionResult> DeleteByKey(string key)
    {
        await gameService.DeleteByKeyAsync(key);
        return Ok();
    }

    [HttpGet("[controller]/{key}/file")]
    public async Task<ActionResult> Download(string key)
    {
        var fileContent = await gameService.GetGameFileAsync(key);

        var fileName = gameService.GetGameFileName(key);

        return File(new MemoryStream(fileContent), "text/plain", fileName);
    }
}
