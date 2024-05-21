namespace PokemonsWebAPI.Accounts;

public class AuthResponse
{
    public string Token { get; set; }

    public int UserID { get; set; }
    public int RoleID { get; set; }
}