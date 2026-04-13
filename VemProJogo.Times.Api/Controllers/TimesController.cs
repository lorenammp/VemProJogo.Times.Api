using Microsoft.AspNetCore.Mvc;
using VemProJogo.Times.Application.Abstractions.Services;
using VemProJogo.Times.Application.DTOs.Times;

namespace VemProJogo.Times.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TimesController : ControllerBase
{
    private readonly ITimesService _timesService;

    public TimesController(ITimesService timesService)
    {
        _timesService = timesService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<TimeResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TimeResponse>>> Get()
    {
        var result = await _timesService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:length(24)}")]
    [ProducesResponseType(typeof(TimeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TimeResponse>> GetById(string id)
    {
        var result = await _timesService.GetByIdAsync(id);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("championship/{championshipId:length(24)}")]
    [ProducesResponseType(typeof(List<TimeResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TimeResponse>>> GetByChampionshipId(string championshipId)
    {
        var result = await _timesService.GetByChampionshipIdAsync(championshipId);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TimeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TimeResponse>> Post([FromBody] CreateTimeRequest request)
    {
        var created = await _timesService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Put(string id, [FromBody] UpdateTimeRequest request)
    {
        await _timesService.UpdateAsync(id, request);
        return NoContent();
    }

    [HttpPatch("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Patch(string id, [FromBody] PatchTimeRequest request)
    {
        await _timesService.PatchAsync(id, request);
        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        await _timesService.DeleteAsync(id);
        return NoContent();
    }
}
