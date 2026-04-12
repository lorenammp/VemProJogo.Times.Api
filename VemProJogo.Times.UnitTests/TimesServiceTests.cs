using VemProJogo.Times.Application.Abstractions.Persistence;
using VemProJogo.Times.Application.DTOs.Estatisticas;
using VemProJogo.Times.Application.DTOs.Jogadores;
using VemProJogo.Times.Application.Services;
using VemProJogo.Times.Domain.Entities;

namespace VemProJogo.Times.UnitTests;

public sealed class TimesServiceTests
{
    [Fact]
    public async Task CreatePlayerAsync_DeveAssociarJogadorAoTime()
    {
        var repository = new FakeTimesRepository(CreateTeam());
        var service = new TimesService(repository);

        var response = await service.CreatePlayerAsync("507f1f77bcf86cd799439011", new CreateJogadorRequest
        {
            Name = "Lucas Lima",
            Number = 10,
            Position = "meia"
        });

        Assert.Equal("Lucas Lima", response.Name);
        Assert.Single(repository.StoredTime!.Players);
        Assert.Equal(10, repository.StoredTime.Players[0].Number);
    }

    [Fact]
    public async Task CreatePlayerStatAsync_DeveRegistrarEstatisticasDaPartida()
    {
        var team = CreateTeam();
        team.Players.Add(new Jogador
        {
            Id = "player-01",
            Name = "Nina",
            Number = 9,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        var repository = new FakeTimesRepository(team);
        var service = new TimesService(repository);

        var response = await service.CreatePlayerStatAsync("507f1f77bcf86cd799439011", "player-01", new CreateEstatisticaJogadorRequest
        {
            MatchId = "507f1f77bcf86cd799439012",
            Goals = 2,
            Assists = 1,
            YellowCards = 0,
            RedCards = 0,
            MinutesPlayed = 90,
            Notes = "Destaque da partida"
        });

        Assert.Equal(2, response.Goals);
        Assert.Single(repository.StoredTime!.Players[0].MatchStats);
        Assert.Equal("507f1f77bcf86cd799439012", repository.StoredTime.Players[0].MatchStats[0].MatchId);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarPlayersNoResponse()
    {
        var team = CreateTeam();
        team.Players.Add(new Jogador
        {
            Id = "player-01",
            Name = "Bia",
            Number = 7,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        var service = new TimesService(new FakeTimesRepository(team));

        var response = await service.GetByIdAsync("507f1f77bcf86cd799439011");

        Assert.NotNull(response);
        Assert.Single(response!.Players);
        Assert.Equal("Bia", response.Players[0].Name);
    }

    private static Time CreateTeam()
    {
        return new Time
        {
            Id = "507f1f77bcf86cd799439011",
            ChampionshipId = "507f1f77bcf86cd799439099",
            Name = "Time Azul",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private sealed class FakeTimesRepository : ITimesRepository
    {
        public FakeTimesRepository(Time? storedTime)
        {
            StoredTime = storedTime;
        }

        public Time? StoredTime { get; private set; }

        public Task<List<Time>> GetAllAsync() =>
            Task.FromResult(StoredTime is null ? new List<Time>() : new List<Time> { StoredTime });

        public Task<Time?> GetByIdAsync(string id) =>
            Task.FromResult(StoredTime?.Id == id ? StoredTime : null);

        public Task<List<Time>> GetByChampionshipIdAsync(string championshipId) =>
            Task.FromResult(StoredTime?.ChampionshipId == championshipId
                ? new List<Time> { StoredTime }
                : new List<Time>());

        public Task CreateAsync(Time time)
        {
            StoredTime = time;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Time time)
        {
            StoredTime = time;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            if (StoredTime?.Id == id)
            {
                StoredTime = null;
            }

            return Task.CompletedTask;
        }
    }
}
