namespace VemProJogo.Times.Application.DTOs.Times;

public class PatchTimeRequest
{
    public string? ChampionshipId { get; set; }
    public string? Name { get; set; }
    public string? Acronym { get; set; }
    public string? ResponsibleName { get; set; }
    public string? ResponsibleContact { get; set; }
    public string? CrestUrl { get; set; }
    public bool? Active { get; set; }
}