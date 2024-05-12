namespace PokemonsDesktop.Models;

public class User
{
    public int Id { get; set; }
    
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }
    
    public bool ChangePassword { get; set; }
}