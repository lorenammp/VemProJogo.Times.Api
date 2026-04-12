namespace VemProJogo.Times.Domain.Entities;

public sealed class Jogador
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Nickname { get; set; }
    public int Number { get; set; }
    public string? Position { get; set; }
    public bool Active { get; set; } = true;
    public List<EstatisticaJogadorPartida> MatchStats { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
