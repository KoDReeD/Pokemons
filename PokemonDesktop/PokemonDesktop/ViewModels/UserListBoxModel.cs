namespace PokemonsDesktop.Models;

public class UserListBoxModel
{
    public int Id { get; set; }
    
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string RoleName { get; set; }
    public int RoleId { get; set; }
    
    public string ChangePassword { get; set; }
}