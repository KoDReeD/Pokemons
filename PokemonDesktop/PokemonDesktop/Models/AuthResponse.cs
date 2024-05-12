namespace PokemonsWebAPI.Models;

public class AuthResponse
{
    public string Token { get; set; }
    public int UserId { get; set; }
    public int RoleId { get; set; }
}