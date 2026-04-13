using System.ComponentModel.DataAnnotations;

namespace VemProJogo.Times.Application.DTOs.Times;

public class PatchTimeRequest
{
    [StringLength(24, MinimumLength = 24)]
    public string? ChampionshipId { get; set; }
    [StringLength(120)]
    public string? Name { get; set; }
    [StringLength(10)]
    public string? Acronym { get; set; }
    [StringLength(120)]
    public string? ResponsibleName { get; set; }
    [StringLength(60)]
    public string? ResponsibleContact { get; set; }
    [StringLength(300)]
    [Url]
    public string? CrestUrl { get; set; }
    public bool? Active { get; set; }
}
