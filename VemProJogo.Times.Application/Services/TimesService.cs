using VemProJogo.Times.Application.Abstractions.Persistence;
using VemProJogo.Times.Application.Abstractions.Services;
using VemProJogo.Times.Application.DTOs.Estatisticas;
using VemProJogo.Times.Application.DTOs.Jogadores;
using VemProJogo.Times.Application.DTOs.Times;
using VemProJogo.Times.Application.Exceptions;
using VemProJogo.Times.Application.Rules;
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
        var normalized = TimeBusinessRules.ValidateAndNormalizeForCreate(request);
        var now = DateTime.UtcNow;

        var time = new Time
        {
            ChampionshipId = normalized.ChampionshipId,
            Name = normalized.Name,
            Acronym = normalized.Acronym,
            ResponsibleName = normalized.ResponsibleName,
            ResponsibleContact = normalized.ResponsibleContact,
            CrestUrl = normalized.CrestUrl,
            Active = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _timesRepository.CreateAsync(time);
        return MapToResponse(time);
    }

    public async Task UpdateAsync(string id, UpdateTimeRequest request)
    {
        var normalized = TimeBusinessRules.ValidateAndNormalizeForUpdate(request);
        var existing = await _timesRepository.GetByIdAsync(id);

        if (existing is null)
            throw new ResourceNotFoundException("Time nao encontrado.");

        existing.ChampionshipId = normalized.ChampionshipId;
        existing.Name = normalized.Name;
        existing.Acronym = normalized.Acronym;
        existing.ResponsibleName = normalized.ResponsibleName;
        existing.ResponsibleContact = normalized.ResponsibleContact;
        existing.CrestUrl = normalized.CrestUrl;
        existing.Active = request.Active;
        existing.UpdatedAt = DateTime.UtcNow;

        await _timesRepository.UpdateAsync(existing);
    }

    public async Task PatchAsync(string id, PatchTimeRequest request)
    {
        var normalized = TimeBusinessRules.ValidateAndNormalizeForPatch(request);
        var existing = await _timesRepository.GetByIdAsync(id);

        if (existing is null)
            throw new ResourceNotFoundException("Time nao encontrado.");

        if (normalized.HasChampionshipId)
            existing.ChampionshipId = normalized.ChampionshipId!;

        if (normalized.HasName)
            existing.Name = normalized.Name!;

        if (normalized.HasAcronym)
            existing.Acronym = normalized.Acronym;

        if (normalized.HasResponsibleName)
            existing.ResponsibleName = normalized.ResponsibleName;

        if (normalized.HasResponsibleContact)
            existing.ResponsibleContact = normalized.ResponsibleContact;

        if (normalized.HasCrestUrl)
            existing.CrestUrl = normalized.CrestUrl;

        if (normalized.HasActive)
            existing.Active = normalized.Active!.Value;

        existing.UpdatedAt = DateTime.UtcNow;

        await _timesRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(string id)
    {
        var existing = await _timesRepository.GetByIdAsync(id);

        if (existing is null)
            throw new ResourceNotFoundException("Time nao encontrado.");

        await _timesRepository.DeleteAsync(id);
    }

    public async Task<List<JogadorResponse>> GetPlayersAsync(string timeId)
    {
        var time = await GetTimeOrThrowAsync(timeId);
        return time.Players.Select(MapPlayerToResponse).ToList();
    }

    public async Task<JogadorResponse?> GetPlayerByIdAsync(string timeId, string playerId)
    {
        var time = await GetTimeOrThrowAsync(timeId);
        var player = FindPlayer(time, playerId);
        return player is null ? null : MapPlayerToResponse(player);
    }

    public async Task<JogadorResponse> CreatePlayerAsync(string timeId, CreateJogadorRequest request)
    {
        ValidatePlayerRequest(request.Name, request.Number);

        var time = await GetTimeOrThrowAsync(timeId);

        EnsurePlayerNumberIsUnique(time, request.Number);

        var now = DateTime.UtcNow;
        var player = new Jogador
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = request.Name.Trim(),
            Nickname = NormalizeOptional(request.Nickname),
            Number = request.Number,
            Position = NormalizeOptional(request.Position),
            Active = true,
            CreatedAt = now,
            UpdatedAt = now
        };

        time.Players.Add(player);
        time.UpdatedAt = now;

        await _timesRepository.UpdateAsync(time);

        return MapPlayerToResponse(player);
    }

    public async Task UpdatePlayerAsync(string timeId, string playerId, UpdateJogadorRequest request)
    {
        ValidatePlayerRequest(request.Name, request.Number);

        var time = await GetTimeOrThrowAsync(timeId);
        var player = GetPlayerOrThrow(time, playerId);

        EnsurePlayerNumberIsUnique(time, request.Number, playerId);

        player.Name = request.Name.Trim();
        player.Nickname = NormalizeOptional(request.Nickname);
        player.Number = request.Number;
        player.Position = NormalizeOptional(request.Position);
        player.Active = request.Active;
        player.UpdatedAt = DateTime.UtcNow;

        time.UpdatedAt = player.UpdatedAt;

        await _timesRepository.UpdateAsync(time);
    }

    public async Task DeletePlayerAsync(string timeId, string playerId)
    {
        var time = await GetTimeOrThrowAsync(timeId);
        var player = GetPlayerOrThrow(time, playerId);

        time.Players.Remove(player);
        time.UpdatedAt = DateTime.UtcNow;

        await _timesRepository.UpdateAsync(time);
    }

    public async Task<List<EstatisticaJogadorPartidaResponse>> GetPlayerStatsAsync(string timeId, string playerId)
    {
        var player = GetPlayerOrThrow(await GetTimeOrThrowAsync(timeId), playerId);
        return player.MatchStats.Select(MapStatToResponse).ToList();
    }

    public async Task<EstatisticaJogadorPartidaResponse> CreatePlayerStatAsync(
        string timeId,
        string playerId,
        CreateEstatisticaJogadorRequest request)
    {
        ValidateStatRequest(request.MatchId, request.Goals, request.Assists, request.YellowCards, request.RedCards, request.MinutesPlayed);

        var time = await GetTimeOrThrowAsync(timeId);
        var player = GetPlayerOrThrow(time, playerId);

        if (player.MatchStats.Any(stat => stat.MatchId.Equals(request.MatchId.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Ja existe estatistica registrada para esse jogador nesta partida.");
        }

        var now = DateTime.UtcNow;
        var stat = new EstatisticaJogadorPartida
        {
            Id = Guid.NewGuid().ToString("N"),
            MatchId = request.MatchId.Trim(),
            Goals = request.Goals,
            Assists = request.Assists,
            YellowCards = request.YellowCards,
            RedCards = request.RedCards,
            MinutesPlayed = request.MinutesPlayed,
            Notes = NormalizeOptional(request.Notes),
            RecordedAt = now,
            UpdatedAt = now
        };

        player.MatchStats.Add(stat);
        player.UpdatedAt = now;
        time.UpdatedAt = now;

        await _timesRepository.UpdateAsync(time);

        return MapStatToResponse(stat);
    }

    public async Task UpdatePlayerStatAsync(
        string timeId,
        string playerId,
        string statId,
        UpdateEstatisticaJogadorRequest request)
    {
        ValidateStatRequest("match-placeholder", request.Goals, request.Assists, request.YellowCards, request.RedCards, request.MinutesPlayed, false);

        var time = await GetTimeOrThrowAsync(timeId);
        var player = GetPlayerOrThrow(time, playerId);
        var stat = GetStatOrThrow(player, statId);

        stat.Goals = request.Goals;
        stat.Assists = request.Assists;
        stat.YellowCards = request.YellowCards;
        stat.RedCards = request.RedCards;
        stat.MinutesPlayed = request.MinutesPlayed;
        stat.Notes = NormalizeOptional(request.Notes);
        stat.UpdatedAt = DateTime.UtcNow;

        player.UpdatedAt = stat.UpdatedAt;
        time.UpdatedAt = stat.UpdatedAt;

        await _timesRepository.UpdateAsync(time);
    }

    public async Task DeletePlayerStatAsync(string timeId, string playerId, string statId)
    {
        var time = await GetTimeOrThrowAsync(timeId);
        var player = GetPlayerOrThrow(time, playerId);
        var stat = GetStatOrThrow(player, statId);

        player.MatchStats.Remove(stat);
        player.UpdatedAt = DateTime.UtcNow;
        time.UpdatedAt = player.UpdatedAt;

        await _timesRepository.UpdateAsync(time);
    }

    private async Task<Time> GetTimeOrThrowAsync(string timeId)
    {
        var time = await _timesRepository.GetByIdAsync(timeId);

        return time ?? throw new ResourceNotFoundException("Time nao encontrado.");
    }

    private static Jogador? FindPlayer(Time time, string playerId) =>
        time.Players.FirstOrDefault(player => player.Id == playerId);

    private static Jogador GetPlayerOrThrow(Time time, string playerId) =>
        FindPlayer(time, playerId) ?? throw new KeyNotFoundException("Jogador nao encontrado.");

    private static EstatisticaJogadorPartida GetStatOrThrow(Jogador player, string statId) =>
        player.MatchStats.FirstOrDefault(stat => stat.Id == statId)
        ?? throw new KeyNotFoundException("Estatistica nao encontrada.");

    private static void ValidatePlayerRequest(string name, int number)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("O nome do jogador e obrigatorio.", nameof(name));
        }

        if (number <= 0)
        {
            throw new ArgumentException("O numero do jogador deve ser maior que zero.", nameof(number));
        }
    }

    private static void ValidateStatRequest(
        string matchId,
        int goals,
        int assists,
        int yellowCards,
        int redCards,
        int minutesPlayed,
        bool validateMatchId = true)
    {
        if (validateMatchId && string.IsNullOrWhiteSpace(matchId))
        {
            throw new ArgumentException("O identificador da partida e obrigatorio.", nameof(matchId));
        }

        var values = new[] { goals, assists, yellowCards, redCards, minutesPlayed };

        if (values.Any(value => value < 0))
        {
            throw new ArgumentException("As estatisticas nao podem ser negativas.");
        }
    }

    private static void EnsurePlayerNumberIsUnique(Time time, int number, string? ignoredPlayerId = null)
    {
        if (time.Players.Any(player => player.Number == number && player.Id != ignoredPlayerId))
        {
            throw new InvalidOperationException("Ja existe um jogador com esse numero no time.");
        }
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

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
            Players = time.Players.Select(MapPlayerToResponse).ToList(),
            CreatedAt = time.CreatedAt,
            UpdatedAt = time.UpdatedAt
        };
    }

    private static JogadorResponse MapPlayerToResponse(Jogador player)
    {
        return new JogadorResponse
        {
            Id = player.Id,
            Name = player.Name,
            Nickname = player.Nickname,
            Number = player.Number,
            Position = player.Position,
            Active = player.Active,
            MatchStats = player.MatchStats.Select(MapStatToResponse).ToList(),
            CreatedAt = player.CreatedAt,
            UpdatedAt = player.UpdatedAt
        };
    }

    private static EstatisticaJogadorPartidaResponse MapStatToResponse(EstatisticaJogadorPartida stat)
    {
        return new EstatisticaJogadorPartidaResponse
        {
            Id = stat.Id,
            MatchId = stat.MatchId,
            Goals = stat.Goals,
            Assists = stat.Assists,
            YellowCards = stat.YellowCards,
            RedCards = stat.RedCards,
            MinutesPlayed = stat.MinutesPlayed,
            Notes = stat.Notes,
            RecordedAt = stat.RecordedAt,
            UpdatedAt = stat.UpdatedAt
        };
    }
}
