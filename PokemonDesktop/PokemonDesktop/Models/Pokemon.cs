namespace PokemonsDesktop.Models;

public class Pokemon
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? PhotoPath { get; set; }

    public int HarakteristiksId { get; set; }

    public int ParametersId { get; set; }
}