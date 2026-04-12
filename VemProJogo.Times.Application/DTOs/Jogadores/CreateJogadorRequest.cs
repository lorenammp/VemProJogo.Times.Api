namespace VemProJogo.Times.Application.DTOs.Jogadores;

public sealed class CreateJogadorRequest
{
    public string Name { get; set; } = null!;
    public string? Nickname { get; set; }
    public int Number { get; set; }
    public string? Position { get; set; }
}
