namespace VemProJogo.Times.Application.DTOs.Estatisticas;

public sealed class EstatisticaJogadorPartidaResponse
{
    public string Id { get; set; } = null!;
    public string MatchId { get; set; } = null!;
    public int Goals { get; set; }
    public int Assists { get; set; }
    public int YellowCards { get; set; }
    public int RedCards { get; set; }
    public int MinutesPlayed { get; set; }
    public string? Notes { get; set; }
    public DateTime RecordedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
