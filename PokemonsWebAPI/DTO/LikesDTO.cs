namespace PokemonsWebAPI.DTO;

public class LikesDTO
{
    public int Id { get; set; }
    public string Pokemon { get; set; }
    public int Star { get; set; }
    public DateOnly DataSet { get; set; }
    public string USername { get; set; }
}