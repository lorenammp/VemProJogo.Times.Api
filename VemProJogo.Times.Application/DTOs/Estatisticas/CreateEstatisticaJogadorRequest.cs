namespace VemProJogo.Times.Application.DTOs.Estatisticas;

public sealed class CreateEstatisticaJogadorRequest
{
    public string MatchId { get; set; } = null!;
    public int Goals { get; set; }
    public int Assists { get; set; }
    public int YellowCards { get; set; }
    public int RedCards { get; set; }
    public int MinutesPlayed { get; set; }
    public string? Notes { get; set; }
}
