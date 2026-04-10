namespace VemProJogo.Times.Domain.Entities;

public sealed class Time
{
    public string? Id { get; set; }
    public string ChampionshipId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Acronym { get; set; }
    public string? ResponsibleName { get; set; }
    public string? ResponsibleContact { get; set; }
    public string? CrestUrl { get; set; }
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}