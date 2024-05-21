using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PokemonsWebAPI.Context;
using PokemonsWebAPI.Models;

namespace PokemonsWebAPI.Accounts.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string GetTokenAsync(User user)
    {
        var jwtKey = _configuration.GetValue<string>("JwtSettings:Secret");
        var key = Encoding.ASCII.GetBytes(jwtKey);
        
        string roleName = Enum.GetName(typeof(RoleEnum), user.RoleId);

        var tokenHandler = new JwtSecurityTokenHandler();
        var descriptor = new SecurityTokenDescriptor()

        {
            //маркеры безопасности
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Role, roleName)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };
        var token = tokenHandler.CreateToken(descriptor);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}

