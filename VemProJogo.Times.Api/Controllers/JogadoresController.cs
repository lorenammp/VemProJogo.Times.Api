using Microsoft.AspNetCore.Mvc;
using VemProJogo.Times.Application.Abstractions.Services;
using VemProJogo.Times.Application.DTOs.Estatisticas;
using VemProJogo.Times.Application.DTOs.Jogadores;

namespace VemProJogo.Times.Api.Controllers;

[ApiController]
[Route("api/times/{timeId:length(24)}/jogadores")]
public sealed class JogadoresController : ControllerBase
{
    private readonly ITimesService _timesService;

    public JogadoresController(ITimesService timesService)
    {
        _timesService = timesService;
    }

    [HttpGet]
    public async Task<ActionResult<List<JogadorResponse>>> Get(string timeId)
    {
        var result = await _timesService.GetPlayersAsync(timeId);
        return Ok(result);
    }

    [HttpGet("{playerId}")]
    public async Task<ActionResult<JogadorResponse>> GetById(string timeId, string playerId)
    {
        var result = await _timesService.GetPlayerByIdAsync(timeId, playerId);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<JogadorResponse>> Post(string timeId, [FromBody] CreateJogadorRequest request)
    {
        var created = await _timesService.CreatePlayerAsync(timeId, request);
        return CreatedAtAction(nameof(GetById), new { timeId, playerId = created.Id }, created);
    }

    [HttpPut("{playerId}")]
    public async Task<IActionResult> Put(string timeId, string playerId, [FromBody] UpdateJogadorRequest request)
    {
        await _timesService.UpdatePlayerAsync(timeId, playerId, request);
        return NoContent();
    }

    [HttpDelete("{playerId}")]
    public async Task<IActionResult> Delete(string timeId, string playerId)
    {
        await _timesService.DeletePlayerAsync(timeId, playerId);
        return NoContent();
    }

    [HttpGet("{playerId}/estatisticas")]
    public async Task<ActionResult<List<EstatisticaJogadorPartidaResponse>>> GetStats(string timeId, string playerId)
    {
        var result = await _timesService.GetPlayerStatsAsync(timeId, playerId);
        return Ok(result);
    }

    [HttpPost("{playerId}/estatisticas")]
    public async Task<ActionResult<EstatisticaJogadorPartidaResponse>> PostStat(
        string timeId,
        string playerId,
        [FromBody] CreateEstatisticaJogadorRequest request)
    {
        var created = await _timesService.CreatePlayerStatAsync(timeId, playerId, request);
        return CreatedAtAction(nameof(GetStats), new { timeId, playerId }, created);
    }

    [HttpPut("{playerId}/estatisticas/{statId}")]
    public async Task<IActionResult> PutStat(
        string timeId,
        string playerId,
        string statId,
        [FromBody] UpdateEstatisticaJogadorRequest request)
    {
        await _timesService.UpdatePlayerStatAsync(timeId, playerId, statId, request);
        return NoContent();
    }

    [HttpDelete("{playerId}/estatisticas/{statId}")]
    public async Task<IActionResult> DeleteStat(string timeId, string playerId, string statId)
    {
        await _timesService.DeletePlayerStatAsync(timeId, playerId, statId);
        return NoContent();
    }
}
