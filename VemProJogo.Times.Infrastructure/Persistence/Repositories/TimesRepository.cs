using MongoDB.Driver;
using VemProJogo.Times.Application.Abstractions.Persistence;
using VemProJogo.Times.Domain.Entities;

namespace VemProJogo.Times.Infrastructure.Persistence.Repositories;

public sealed class TimesRepository : ITimesRepository
{
    private readonly MongoDbContext _context;

    public TimesRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<List<Time>> GetAllAsync() =>
        await _context.Times.Find(_ => true).ToListAsync();

    public async Task<Time?> GetByIdAsync(string id) =>
        await _context.Times.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<List<Time>> GetByChampionshipIdAsync(string championshipId) =>
        await _context.Times.Find(x => x.ChampionshipId == championshipId).ToListAsync();

    public async Task CreateAsync(Time time) =>
        await _context.Times.InsertOneAsync(time);

    public async Task UpdateAsync(Time time) =>
        await _context.Times.ReplaceOneAsync(x => x.Id == time.Id, time);

    public async Task DeleteAsync(string id) =>
        await _context.Times.DeleteOneAsync(x => x.Id == id);
}