namespace PokemonsWebAPI.DTO;

public class UserDTO
{
    public int Id { get; set; }
    
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public string RoleName { get; set; }
    
    public string ChangePassword { get; set; }
}