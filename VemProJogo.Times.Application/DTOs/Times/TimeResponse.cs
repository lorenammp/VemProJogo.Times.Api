using VemProJogo.Times.Application.DTOs.Jogadores;

namespace VemProJogo.Times.Application.DTOs.Times;

public class TimeResponse
{
    public string Id { get; set; } = null!;
    public string ChampionshipId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Acronym { get; set; }
    public string? ResponsibleName { get; set; }
    public string? ResponsibleContact { get; set; }
    public string? CrestUrl { get; set; }
    public bool Active { get; set; }
    public List<JogadorResponse> Players { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
