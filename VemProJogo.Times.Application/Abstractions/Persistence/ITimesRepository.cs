using VemProJogo.Times.Domain.Entities;

namespace VemProJogo.Times.Application.Abstractions.Persistence;

public interface ITimesRepository
{
    Task<List<Time>> GetAllAsync();
    Task<Time?> GetByIdAsync(string id);
    Task<List<Time>> GetByChampionshipIdAsync(string championshipId);
    Task CreateAsync(Time time);
    Task UpdateAsync(Time time);
    Task DeleteAsync(string id);
}