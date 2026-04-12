using VemProJogo.Times.Application.DTOs.Estatisticas;
using VemProJogo.Times.Application.DTOs.Jogadores;
using VemProJogo.Times.Application.DTOs.Times;

namespace VemProJogo.Times.Application.Abstractions.Services;

public interface ITimesService
{
    Task<List<TimeResponse>> GetAllAsync();
    Task<TimeResponse?> GetByIdAsync(string id);
    Task<List<TimeResponse>> GetByChampionshipIdAsync(string championshipId);
    Task<TimeResponse> CreateAsync(CreateTimeRequest request);
    Task UpdateAsync(string id, UpdateTimeRequest request);
    Task PatchAsync(string id, PatchTimeRequest request);
    Task DeleteAsync(string id);
    Task<List<JogadorResponse>> GetPlayersAsync(string timeId);
    Task<JogadorResponse?> GetPlayerByIdAsync(string timeId, string playerId);
    Task<JogadorResponse> CreatePlayerAsync(string timeId, CreateJogadorRequest request);
    Task UpdatePlayerAsync(string timeId, string playerId, UpdateJogadorRequest request);
    Task DeletePlayerAsync(string timeId, string playerId);
    Task<List<EstatisticaJogadorPartidaResponse>> GetPlayerStatsAsync(string timeId, string playerId);
    Task<EstatisticaJogadorPartidaResponse> CreatePlayerStatAsync(string timeId, string playerId, CreateEstatisticaJogadorRequest request);
    Task UpdatePlayerStatAsync(string timeId, string playerId, string statId, UpdateEstatisticaJogadorRequest request);
    Task DeletePlayerStatAsync(string timeId, string playerId, string statId);
}
