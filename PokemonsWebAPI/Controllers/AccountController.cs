using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PokemonsWebAPI.Accounts;
using PokemonsWebAPI.Accounts.Services;
using PokemonsWebAPI.Context;
using PokemonsWebAPI.Models;

namespace PokemonsWebAPI.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AccountController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly MgssswnzContext _db;

    public AccountController(IJwtService jwtService, MgssswnzContext context)
    {
        _jwtService = jwtService;
        _db = context;
    }

    
    [HttpPost]
    public async Task<IActionResult> Authorization([FromBody] AuthRequest authRequest)
    {
        string username = authRequest.UserName;
        string password = authRequest.Password;

        if (string.IsNullOrWhiteSpace(password))
        {
            return BadRequest("Заполните пароль");
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            return BadRequest("Заполните username");
        }

        string passwordHash = PasswordHash(password);

        User user = await _db.Users.Where(x => x.Username == username && x.Password == passwordHash)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return BadRequest("Ошибка входа");
        }

        if (user.PasswordChange == true && user.RoleId != 1)
        {
            return Conflict(user.RoleId);
        }

        var token = _jwtService.GetTokenAsync(user);

        if (token == null)
        {
            return Unauthorized();
        }

        return Ok(new AuthResponse
        {
            Token = token, UserID = user.Id, RoleID = user.RoleId
        });
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> PasswordReset(int userId)
    {
        var user = await _db.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
        if (user == null)
        {
            return BadRequest("Пользователь не найден");
        }

        if (user.RoleId == 1)
        {
            return BadRequest("Нельзя сбросить пароль админу");
        }

        if (user.PasswordChange == true)
        {
            return Conflict();
        }

        user.PasswordChange = true;

        try
        {
            _db.Users.Update(user);
            _db.SaveChanges();

            var token = _jwtService.GetTokenAsync(user);

            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(new AuthResponse
            {
                Token = token, UserID = user.Id, RoleID = user.RoleId
            });
        }
        catch (Exception e)
        {
            return BadRequest("Ошибка сохранения");
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ChangePassword([FromBody] AuthRequest authRequest)
    {
        var user = await _db.Users.Where(x => x.Username == authRequest.UserName).FirstOrDefaultAsync();
        if (user == null)
        {
            return BadRequest("Пользователь не найден");
        }

        if (!user.PasswordChange == true)
        {
            return BadRequest("Вам не меняли пароль");
        }

        string passHash = PasswordHash(authRequest.Password);
        user.Password = passHash;
        user.PasswordChange = false;

        try
        {
            _db.Users.Update(user);
            _db.SaveChanges();
            return Ok("Пароль изменён");
        }
        catch (Exception e)
        {
            return BadRequest("Ошибка сохранения");
        }
    }

    
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

    [HttpPost]
    public async Task<IActionResult> Registration([FromBody] AuthRequest authRequest)
    {
        string username = authRequest.UserName;
        string password = authRequest.Password;

        if (string.IsNullOrWhiteSpace(password))
        {
            return BadRequest("Заполните пароль");
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            return BadRequest("Заполните username");
        }

        var checkUser = await _db.Users.Where(x => x.Username == username).FirstOrDefaultAsync();

        if (checkUser != null)
        {
            return BadRequest("Логин занят");
        }

        string hashPass = PasswordHash(password);

        User user = new User()
        {
            Password = hashPass,
            Username = username,
            RoleId = 2,
            PasswordChange = true
        };

        _db.Users.Add(user);
        try
        {
            _db.SaveChanges();
            return Ok("Пользователь добавлен");
        }
        catch (Exception e)
        {
            return BadRequest("Ошибка добавления");
        }
    }

    [Authorize]
    private async Task<IActionResult> SetActivity(UserActivityEnum activityId, int userId, DateTime date)
    {
        if (userId == 0)
        {
            return BadRequest("Пользователь не найден");
        }

        if (date > DateTime.Now)
        {
            return BadRequest("Дата не верная");
        }

        var userActivity = new UserActivity();
        userActivity.UserId = userId;
        userActivity.Date = date;
        userActivity.ActivityId = Convert.ToInt32(activityId);

        _db.UserActivities.Add(userActivity);
        try
        {
            _db.SaveChanges();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest("Ошибка сохранения");
        }
    }

    public class ActivityClass
    {
        public int UserId { get; set; }
        public DateTime DateTime { get; set; }
    }
    
    [Authorize]
    [HttpPost]
    public Task<IActionResult> SignIn([FromBody] ActivityClass activityClass)
    {
        return SetActivity(UserActivityEnum.Entry, activityClass.UserId, activityClass.DateTime);
    }

    [Authorize]
    [HttpPost]
    public Task<IActionResult> SignOut([FromBody] ActivityClass activityClass)
    {
        return SetActivity(UserActivityEnum.Exit, activityClass.UserId, activityClass.DateTime);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetActivityByUserId(int page, int count, int userId)
    {
        if (page <= 0)
        {
            return BadRequest("Неверная страница");
        }

        if (count <= 0)
        {
            return BadRequest("Неверное количество");
        }

        if (userId <= 0)
        {
            return BadRequest("Пользователь не найден");
        }

        int length = _db.UserActivities.Where(x => x.UserId == userId).Count();

        if (length == 0)
        {
            return NoContent();
        }

        var list = await _db.UserActivities
            .Where(x => x.UserId == userId)
            .OrderBy(o => o.Date)
            .Include(i => i.UserNavigation)
            .Include(i => i.Activity)
            .Select(x => new
            {
                x.Id,
                x.UserNavigation.Username,
                Activity = x.Activity.Title,
                x.Date
            })
            .Skip((page - 1) * count)
            .Take(count)
            .ToListAsync();

        if (list.Count() == 0)
        {
            return BadRequest("Неверная страница");
        }

        return Ok(list);
    }
}