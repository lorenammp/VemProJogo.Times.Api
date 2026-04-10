namespace VemProJogo.Times.Application.DTOs.Times;

public class CreateTimeRequest
{
    public string ChampionshipId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Acronym { get; set; }
    public string? ResponsibleName { get; set; }
    public string? ResponsibleContact { get; set; }
    public string? CrestUrl { get; set; }
}