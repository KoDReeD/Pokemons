using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokemonsWebAPI.Context;
using PokemonsWebAPI.DTO;
using PokemonsWebAPI.Models;

namespace PokemonsWebAPI.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UserController : ControllerBase
{
    private readonly MgssswnzContext _db;

    public UserController(MgssswnzContext db)
    {
        _db = db;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllByPage(int page, int count, bool sortByASC)
    {
        //  Валидация данных
        if (page <= 0)
        {
            return BadRequest("Страница указана не корректно");
        }
        
        if (count <= 0)
        {
            return BadRequest("Количество указано не корректно");
        }
        
        int allRows = _db.Users.Count();

        if (allRows == 0)
        {
            return NoContent();
        }

        if (Math.Ceiling((double)allRows / count) < page)
        {
            return BadRequest("Такой страницы нет");
        }

        List<UserDTO> users;

        if (sortByASC)
        {
            users = await _db.Users
                .OrderBy(o => o.Username)
                .Include(i => i.Role)
                .Select(x => new UserDTO()
                {
                    Id = x.Id,
                    Username = x.Username,
                    Password = x.Password,
                    RoleName = x.Role.Title,
                    RoleId = x.RoleId,
                    ChangePassword = x.PasswordChange == true ? "Пароль был изменён" : ""
                })
                .ToListAsync();
        }
        else
        {
            users = await _db.Users
                .OrderByDescending(o => o.Username)
                .Include(i => i.Role)
                .Select(x => new UserDTO()
                {
                    Id = x.Id,
                    Username = x.Username,
                    Password = x.Password,
                    RoleName = x.Role.Title,
                    RoleId = x.RoleId,
                    ChangePassword = x.PasswordChange == true ? "Пароль был изменён" : ""
                })
                .ToListAsync();
        }

        users = users
            .Skip((page - 1) * count)
            .Take(count).ToList();

        if (users.Count == 0)
        {
            return BadRequest("Такой страницы нет");
        }

        return Ok(users);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllByPageByUsername(int page, int count, bool sortByASC, string title)
    {
        int allRows = _db.Users.Count();
        
        if (allRows == 0)
        {
            return NoContent();
        }
        
        //  Валидация данных
        if (page <= 0)
        {
            return BadRequest("Страница указана не корректно");
        }

        if (Math.Ceiling((double)allRows / count) < page)
        {
            return BadRequest("Такой страницы нет");
        }

        if (count <= 0)
        {
            return BadRequest("Количество указано не корректно");
        }

        var users = await _db.Users
            .Where(x => x.Username.ToLower().Contains(title.ToLower()))
            .Select(x => new UserDTO()
            {
                Id = x.Id,
                Username = x.Username,
                Password = x.Password,
                RoleName = x.Role.Title,
                RoleId = x.RoleId,
                ChangePassword = x.PasswordChange == true ? "Пароль был изменён" : ""
            })
            .ToListAsync();


        if (users.Count == 0)
        {
            return NoContent();
        }

        if (sortByASC)
        {
            users = users.OrderBy(o => o.Username).ToList();
        }
        else
        {
            users = users.OrderByDescending(o => o.Username).ToList();
        }

        users = users
            .Skip((page - 1) * count)
            .Take(count).ToList();

        if (users.Count == 0)
        {
            return BadRequest("Такой страницы нет");
        }

        return Ok(users);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _db.UserRoles.ToListAsync();
        return Ok(roles);
    }

    [Authorize(Roles = "Admin")]
    private string PasswordHash(string password)
    {
        string hashPass = "";
        
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] data = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }
            hashPass = builder.ToString();
        }

        return hashPass;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddUser(string login, string pass, int roleId)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(pass) || roleId <= 0)
        {
            return BadRequest("Данные не корректны");
        }
        
        User checkUser = await _db.Users.Where(x => x.Username == login).FirstOrDefaultAsync();
        if (checkUser != null)
        {
            return BadRequest("Логин занят");
        }

        string hashPass = PasswordHash(pass);
        
        var userForAdd = new User()
        {
            Username = login,
            Password = hashPass,
            RoleId = roleId
        };

        try
        {
            _db.Users.Add(userForAdd);
            _db.SaveChanges();
            return Ok("Пользователь добавлен");
        }
        catch (Exception e)
        {
            return BadRequest("Ошибка добавления в бд");
        }
    }

}