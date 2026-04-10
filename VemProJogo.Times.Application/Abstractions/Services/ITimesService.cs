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
}