using VemProJogo.Times.Application.DTOs.Estatisticas;

namespace VemProJogo.Times.Application.DTOs.Jogadores;

public sealed class JogadorResponse
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Nickname { get; set; }
    public int Number { get; set; }
    public string? Position { get; set; }
    public bool Active { get; set; }
    public List<EstatisticaJogadorPartidaResponse> MatchStats { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
