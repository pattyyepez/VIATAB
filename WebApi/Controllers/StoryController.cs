using DTOs;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("api/stories")]
public class StoryController : ControllerBase
{
    private readonly IStoryService _service;
    private readonly ILogger<StoryController> _logger;

    public StoryController(IStoryService service, ILogger<StoryController> logger)
    {
        _service = service;
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateStoryDto story)
    {
        var response = await _service.AddAsync(story);
        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        List<StoryDto> list = await _service.GetAll();
        return Ok(list);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        return Ok(await _service.GetByIdAsync(id));
    }
    
    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStoryDto story)
    {
        return Ok(await _service.UpdateAsync(id, story));
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteByIdAsync(id);
        return Ok();
    }
}