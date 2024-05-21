using PokemonsWebAPI.Models;

namespace PokemonsWebAPI.Accounts.Services;

public interface IJwtService
{
    string GetTokenAsync(User user);
}