using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokemonsWebAPI.Context;
using PokemonsWebAPI.DTO;

namespace PokemonsWebAPI.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class LikeController : ControllerBase
{
    private readonly MgssswnzContext _db;

    public LikeController(MgssswnzContext db)
    {
        _db = db;
    }

    //все оценки пользователя
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetByUser(int userId, int page, int count)
    {
        int allRows = _db.Raitings.Where(x => x.UserId == userId).Count();

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

        var raiting = await _db.Raitings.Include(u => u.User)
            .Include(p => p.Pokemon)
            .Include(p => p.User)
            .Where(x => x.UserId == userId)
            .OrderBy(o => o.Pokemon.Title)
            .Select(x => new
            {
                x.Pokemon.Title,
                x.Star,
                x.DataSet,
                x.User.Username
            })
            .ToListAsync();

        if (raiting.Count() == 0)
        {
            return NoContent();
        }
        
        raiting = raiting
            .Skip((page - 1) * count)
            .Take(count).ToList();
        
        if (raiting.Count() == 0)
        {
            return BadRequest("Такой страницы нет");
        }


        return Ok(raiting);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllPageByTitle(int page, int count, bool orderByASC, string text)
    {
        if (page <= 0)
        {
            return BadRequest("Неверная страница");
        }

        if (count <= 0)
        {
            return BadRequest("Неверное количество");
        }

        int length = _db.Raitings.Count();

        if (length == 0)
        {
            return NoContent();
        }

        var list = await _db.Raitings
            .Include(i => i.User)
            .Include(i => i.Pokemon)
            .Where(x => x.User.Username.ToLower().Contains(text.ToLower()) ||
                        x.Pokemon.Title.ToLower().Contains(text.ToLower()))
            .Select(x => new
            {
                x.Id,
                Pokemon = x.Pokemon.Title,
                x.Star,
                x.DataSet,
                Username = x.User.Username,
            })
            .ToListAsync();

        if (list.Count() == 0)
        {
            return NoContent();
        }

        if (orderByASC)
        {
            list = list.OrderBy(x => x.DataSet).ToList();
        }
        else
        {
            list = list.OrderByDescending(x => x.DataSet).ToList();
        }

        list = list
            .Skip((page - 1) * count)
            .Take(count)
            .ToList();

        if (list.Count() == 0)
        {
            return BadRequest("Неверная страница");
        }

        return Ok(list);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllByPage(int page, int count, bool orderByASC)
    {
        if (page <= 0)
        {
            return BadRequest("Неверная страница");
        }

        if (count <= 0)
        {
            return BadRequest("Неверное количество");
        }

        int length = _db.Raitings.Count();

        if (length == 0)
        {
            return NoContent();
        }

        List<LikesDTO> list = new List<LikesDTO>();

        if (orderByASC)
        {
            list = await _db.Raitings
                .Include(i => i.User)
                .Include(i => i.Pokemon)
                .OrderBy(x => x.DataSet)
                .Select(x => new LikesDTO
                {
                    Id = x.Id,
                    Pokemon = x.Pokemon.Title,
                    Star = x.Star,
                    DataSet = x.DataSet,
                    USername = x.User.Username,
                }).ToListAsync();
        }
        else
        {
            list = await _db.Raitings
                .Include(i => i.User)
                .Include(i => i.Pokemon)
                .OrderByDescending(x => x.DataSet)
                .Select(x => new LikesDTO
                {
                    Id = x.Id,
                    Pokemon = x.Pokemon.Title,
                    Star = x.Star,
                    DataSet = x.DataSet,
                    USername = x.User.Username,
                }).ToListAsync();
        }

        list = list
            .Skip((page - 1) * count)
            .Take(count)
            .ToList();

        if (list.Count() == 0)
        {
            return BadRequest("Неверная страница");
        }

        return Ok(list);
    }
}