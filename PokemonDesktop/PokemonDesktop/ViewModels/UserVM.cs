namespace PokemonsDesktop.Models;

public class UserVM
{
    public string Username { get; set; }
    public string Password { get; set; }
    public UserRole SelectedRole { get; set; }
}