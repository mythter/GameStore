using BusinessLogic.Requests.Genre;
using BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers;

[Route("api/")]
[ApiController]
public class GenresController(IGenreService genreService) : ControllerBase
{
    [HttpGet("[controller]/{id}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        var genre = await genreService.GetByIdAsync(id);
        return Ok(genre);
    }

    [HttpGet("[controller]")]
    public async Task<ActionResult> GetAll()
    {
        var genres = await genreService.GetAll().ToListAsync();
        return Ok(genres);
    }

    [HttpGet("[controller]/{id}/genres")]
    public async Task<ActionResult> GetAllByParentId(Guid id)
    {
        var genres = await genreService.GetAllByParentId(id).ToListAsync();
        return Ok(genres);
    }

    [HttpGet("games/{key}/genres")]
    public async Task<ActionResult> GetGenresByGameKey(string key)
    {
        var genres = await genreService.GetAllByGameKey(key);
        return Ok(await genres.ToListAsync());
    }

    [HttpPost("[controller]")]
    public async Task<ActionResult> Create([FromBody] GenreCreateRequest request)
    {
        var gameDto = await genreService.AddAsync(request);
        return Ok(gameDto);
    }

    [HttpPut("[controller]")]
    public ActionResult Update([FromBody] GenreUpdateRequest request)
    {
        genreService.UpdateAsync(request);
        return Ok();
    }

    [HttpDelete("[controller]/{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await genreService.DeleteAsync(id);
        return Ok();
    }
}
