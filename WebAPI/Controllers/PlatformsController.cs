using BusinessLogic.Requests.Platform;
using BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers;

[Route("api/")]
[ApiController]
public class PlatformsController(IPlatformService platformService) : ControllerBase
{
    [HttpGet("[controller]/{id}")]
    public async Task<ActionResult> GetById(Guid id)
    {
        var genre = await platformService.GetByIdAsync(id);
        return Ok(genre);
    }

    [HttpGet("[controller]")]
    public async Task<ActionResult> GetAll()
    {
        var genres = await platformService.GetAll().ToListAsync();
        return Ok(genres);
    }

    [HttpGet("games/{key}/platforms")]
    public async Task<ActionResult> GetPlatformsByGameKey(string key)
    {
        var platforms = await platformService.GetAllByGameKey(key);
        return Ok(await platforms.ToListAsync());
    }

    [HttpPost("[controller]")]
    public async Task<ActionResult> Create([FromBody] PlatformCreateRequest request)
    {
        var gameDto = await platformService.AddAsync(request);
        return Ok(gameDto);
    }

    [HttpPut("[controller]")]
    public ActionResult Update([FromBody] PlatformUpdateRequest request)
    {
        platformService.UpdateAsync(request);
        return Ok();
    }

    [HttpDelete("[controller]/{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await platformService.DeleteAsync(id);
        return Ok();
    }
}
