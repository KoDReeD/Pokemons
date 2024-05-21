namespace PokemonsWebAPI.DTO;

public class PokemonAddDTO
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string PhotoPath { get; set; } = null!;

    public int ParameterId { get; set; }

    public int HarakteristikId { get; set; }
    
    
}