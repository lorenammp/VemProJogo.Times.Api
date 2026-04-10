using VemProJogo.Times.Application.Abstractions.Persistence;
using VemProJogo.Times.Application.Abstractions.Services;
using VemProJogo.Times.Application.DTOs.Times;
using VemProJogo.Times.Domain.Entities;

namespace VemProJogo.Times.Application.Services;

public sealed class TimesService : ITimesService
{
    private readonly ITimesRepository _timesRepository;

    public TimesService(ITimesRepository timesRepository)
    {
        _timesRepository = timesRepository;
    }

    public async Task<List<TimeResponse>> GetAllAsync()
    {
        var times = await _timesRepository.GetAllAsync();
        return times.Select(MapToResponse).ToList();
    }

    public async Task<TimeResponse?> GetByIdAsync(string id)
    {
        var time = await _timesRepository.GetByIdAsync(id);
        return time is null ? null : MapToResponse(time);
    }

    public async Task<List<TimeResponse>> GetByChampionshipIdAsync(string championshipId)
    {
        var times = await _timesRepository.GetByChampionshipIdAsync(championshipId);
        return times.Select(MapToResponse).ToList();
    }

    public async Task<TimeResponse> CreateAsync(CreateTimeRequest request)
    {
        var now = DateTime.UtcNow;

        var time = new Time
        {
            ChampionshipId = request.ChampionshipId,
            Name = request.Name,
            Acronym = request.Acronym,
            ResponsibleName = request.ResponsibleName,
            ResponsibleContact = request.ResponsibleContact,
            CrestUrl = request.CrestUrl,
            Active = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _timesRepository.CreateAsync(time);
        return MapToResponse(time);
    }

    public async Task UpdateAsync(string id, UpdateTimeRequest request)
    {
        var existing = await _timesRepository.GetByIdAsync(id);

        if (existing is null)
            throw new KeyNotFoundException("Time não encontrado.");

        existing.ChampionshipId = request.ChampionshipId;
        existing.Name = request.Name;
        existing.Acronym = request.Acronym;
        existing.ResponsibleName = request.ResponsibleName;
        existing.ResponsibleContact = request.ResponsibleContact;
        existing.CrestUrl = request.CrestUrl;
        existing.Active = request.Active;
        existing.UpdatedAt = DateTime.UtcNow;

        await _timesRepository.UpdateAsync(existing);
    }

    public async Task PatchAsync(string id, PatchTimeRequest request)
    {
        var existing = await _timesRepository.GetByIdAsync(id);

        if (existing is null)
            throw new KeyNotFoundException("Time não encontrado.");

        if (request.ChampionshipId is not null)
            existing.ChampionshipId = request.ChampionshipId;

        if (request.Name is not null)
            existing.Name = request.Name;

        if (request.Acronym is not null)
            existing.Acronym = request.Acronym;

        if (request.ResponsibleName is not null)
            existing.ResponsibleName = request.ResponsibleName;

        if (request.ResponsibleContact is not null)
            existing.ResponsibleContact = request.ResponsibleContact;

        if (request.CrestUrl is not null)
            existing.CrestUrl = request.CrestUrl;

        if (request.Active.HasValue)
            existing.Active = request.Active.Value;

        existing.UpdatedAt = DateTime.UtcNow;

        await _timesRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(string id)
    {
        var existing = await _timesRepository.GetByIdAsync(id);

        if (existing is null)
            throw new KeyNotFoundException("Time não encontrado.");

        await _timesRepository.DeleteAsync(id);
    }

    private static TimeResponse MapToResponse(Time time)
    {
        return new TimeResponse
        {
            Id = time.Id ?? string.Empty,
            ChampionshipId = time.ChampionshipId,
            Name = time.Name,
            Acronym = time.Acronym,
            ResponsibleName = time.ResponsibleName,
            ResponsibleContact = time.ResponsibleContact,
            CrestUrl = time.CrestUrl,
            Active = time.Active,
            CreatedAt = time.CreatedAt,
            UpdatedAt = time.UpdatedAt
        };
    }
}