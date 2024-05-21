using System.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using PokemonsWebAPI.Context;
using PokemonsWebAPI.DTO;
using PokemonsWebAPI.Models;

namespace PokemonsWebAPI.Controllers;

// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("[controller]/[action]")]
public class PokemonController : ControllerBase
{
    private readonly MgssswnzContext _db;

    public PokemonController(MgssswnzContext db)
    {
        _db = db;
    }

    //  ПОЛУЧЕНИЕ СПИСКА ВСЕХ ПОКЕМОН
    [Authorize]
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

        int allRows = _db.Pokemons.Count();

        if (allRows == 0)
        {
            return NoContent();
        }

        if (Math.Ceiling((double)allRows / count) < page)
        {
            return BadRequest("Такой страницы нет");
        }

        List<PokemonDTO> pokemonPage;

        if (sortByASC)
        {
            pokemonPage = await _db.Pokemons
                .OrderBy(o => o.Id)
                .Select(x => new PokemonDTO()
                {
                    Id = x.Id,
                    Title = x.Title,
                    PhotoPath = x.PhotoPath
                })
                .ToListAsync();
        }
        else
        {
            pokemonPage = await _db.Pokemons
                .OrderByDescending(o => o.Id)
                .Select(x => new PokemonDTO()
                {
                    Id = x.Id,
                    Title = x.Title,
                    PhotoPath = x.PhotoPath
                })
                .ToListAsync();
        }

        pokemonPage = pokemonPage
            .Skip((page - 1) * count)
            .Take(count).ToList();

        if (pokemonPage.Count == 0)
        {
            return BadRequest("Страница не найдена");
        }

        return Ok(pokemonPage);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllByPageAndTitle(int page, int count, bool sortByASC, string title)
    {
        int allRows = _db.Pokemons.Count();

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

        var pokemonPage = await _db.Pokemons
            .Where(x => x.Title.ToLower().Contains(title.ToLower()) || x.Id.ToString().Contains(title))
            .Select(x => new
            {
                x.Id,
                x.Title,
                x.PhotoPath
            }).ToListAsync();

        if (pokemonPage.Count == 0)
        {
            return NoContent();
        }

        if (sortByASC)
        {
            pokemonPage = pokemonPage.OrderBy(o => o.Id).ToList();
        }
        else
        {
            pokemonPage = pokemonPage.OrderByDescending(o => o.Id).ToList();
        }

        pokemonPage = pokemonPage
            .Skip((page - 1) * count)
            .Take(count)
            .ToList();

        if (pokemonPage.Count == 0)
        {
            return BadRequest("Такой страницы нет");
        }

        return Ok(pokemonPage);
    }

    //  ПОЛУЧЕНИЕ ВСЕХ ПАРАМЕТРОВ ПОКЕМОНОВ
    [HttpGet]
    public async Task<IActionResult> GetParametersPokemons()
    {
        var pokemonParametersList = await _db.PokemonParameters
            .OrderBy(o => o.Id)
            .Include(i => i.ExpGroupNavigation)
            .Select(x => new
            {
                x.Id,
                x.Height,
                x.Weigh,
                x.AllInGame,
                x.Shainy,
                x.Rarity,
                x.HatchingTime,
                x.ExpGroup,
                x.ExpGroupNavigation.Title,
                x.EvolutionStage
            })
            .ToListAsync();

        return Ok(pokemonParametersList);
    }

    [HttpGet]
    public async Task<IActionResult> GetParametersPokemonsById(int pokemonId)
    {
        var pokemon = await _db.Pokemons.Where(x => x.Id == pokemonId).FirstOrDefaultAsync();

        if (pokemon == null)
        {
            return BadRequest("Такого покемона нет");
        }

        int paramId = pokemon.ParameterId;

        var parameters = await _db.PokemonParameters
            .Where(x => x.Id == paramId)
            .Include(t => t.ExpGroupNavigation)
            .Select(x => new
            {
                x.Id,
                x.Height,
                x.Weigh,
                x.AllInGame,
                x.Shainy,
                x.Rarity,
                x.HatchingTime,
                x.ExpGroup,
                x.ExpGroupNavigation.Title,
                x.EvolutionStage
            })
            .ToListAsync();
        return Ok(parameters);
    }

    //  ПОЛУЧЕНИЕ ВСЕХ ХАРАКТЕРИСТИК ПОКЕМОНОВ
    [HttpGet]
    public async Task<IActionResult> GetHarakteristiksPokemons()
    {
        var pokemonHaracteristicsList = await _db.PokemonHarakteristiks
            .OrderBy(o => o.Id)
            .Select(x => new
            {
                x.Id,
                x.Healt,
                x.Attack,
                x.Protect,
                x.SpecAttack,
                x.SpecProtect,
                x.Speed,
                x.Sum
            }).ToListAsync();

        return Ok(pokemonHaracteristicsList);
    }

    [HttpGet]
    public async Task<IActionResult> GetHarakteristiksPokemonsById(int pokId)
    {
        var pokemon = await _db.Pokemons.Where(x => x.Id == pokId).FirstOrDefaultAsync();

        if (pokemon == null)
        {
            return BadRequest("Такого покемона нет");
        }

        int paramId = pokemon.HarakteristikId;

        var harakteristiks = await _db.PokemonHarakteristiks
            .Where(x => x.Id == paramId)
            .Select(x => new
            {
                x.Id,
                x.Healt,
                x.Attack,
                x.Protect,
                x.SpecAttack,
                x.SpecProtect,
                x.Speed,
                x.Sum
            }).ToListAsync();

        return Ok(harakteristiks);
    }

    [HttpGet]
    public async Task<IActionResult> FoundPokemon(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var pokemon = await _db.Pokemons.Where(x => x.Id == id).FirstOrDefaultAsync();

        if (pokemon == null)
        {
            return BadRequest("Покемон не найден");
        }
        return Ok(pokemon);
        
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> FoundPokemonFullInfo(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        var pokemon = await _db.Pokemons.Where(x => x.Id == id)
            .Include(x => x.Harakteristik)
            .Include(x => x.Parameter)
            .Include(x => x.Parameter.ExpGroupNavigation)
            .Select(x => new
            {
                x.Id,
                x.Title,
                x.PhotoPath,
                x.Harakteristik.Attack,
                x.Harakteristik.Healt,
                x.Harakteristik.Speed,
                x.Harakteristik.Protect,
                x.Harakteristik.Sum,
                x.Harakteristik.SpecAttack,
                x.Harakteristik.SpecProtect,
                x.Parameter.Height,
                x.Parameter.Weigh,
                Group = x.Parameter.ExpGroupNavigation.Title,
                x.Parameter.Rarity,
                x.Parameter.Shainy,
                x.Parameter.HatchingTime,
                x.Parameter.AllInGame,
                x.Parameter.EvolutionStage
            })
            .FirstOrDefaultAsync();

        if (pokemon == null)
        {
            return BadRequest("Покемон не найден");
        }
        
        return Ok(pokemon);
        
    }

    //  ПОЛУЧЕНИЕ ВСЕХ ИЗОБРАЖЕНИЙ ПОКЕМОНОВ
    [HttpGet]
    public async Task<IActionResult> GetImagePokemons()
    {
        var pokemonImageList = await _db.Pokemons.Select(x => new
        {
            x.Id,
            x.Title,
            x.PhotoPath
        }).ToListAsync();
        return Ok(pokemonImageList.OrderBy(x => x.Id));
    }

    [HttpGet]
    public async Task<IActionResult> GetPokemonEvolutionStage(int pokemonID)
    {
        var pokemon = await _db.PokemonParameters.FindAsync(pokemonID);
        if (pokemon == null)
        {
            return NotFound("Покемон не найден, проверьте корректность ID");
        }

        return Ok(pokemon.EvolutionStage);
    }

    //  УСТАНОВКА ОЦЕНКИ ПОКЕМОНУ
    [HttpPost]
    public async Task<IActionResult> SetPokemonStar(int userId, int pokemonId, int starCount, DateOnly dateSet)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        if (today < dateSet)
        {
            return BadRequest("Не корректная дата");
        }

        var user = _db.Users.Find(userId);
        if (user == null) return BadRequest("Пользователь не найден");

        if (starCount <= 0 || starCount > 5) return BadRequest("Количество звёзд указано неверно(1-5)");


        var checkRaiting = await _db.Raitings.Where(x => x.UserId == userId && x.PokemonId == pokemonId)
            .FirstOrDefaultAsync();

        if (checkRaiting != null)
        {
            return BadRequest("Вы уже оценивали этого покемона");
        }

        Raiting raiting = new Raiting()
        {
            UserId = userId,
            PokemonId = pokemonId,
            Star = starCount,
            DataSet = dateSet
        };

        _db.Raitings.Add(raiting);
        try
        {
            _db.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return BadRequest("Ошибка сохранения данных в бд");
        }

        return Ok(raiting);
    }

    //  ПОЛУЧЕНИЕ ПОКЕМОНА ДНЯ
    [HttpGet]
    public async Task<IActionResult> GetPokemonDay()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        //находим информацию в бд о покемоне дня
        var row = await _db.PokemonActuals.FirstOrDefaultAsync(x => x.Value == "day" && x.LastDayActual == today);
        int randId = 0;

        //если такой строки нет, то создаётся
        if (row == null)
        {
            //рандомное значение будет генерироваться пока не будет подходящим
            while (true)
            {
                var maxId = await _db.Pokemons.MaxAsync(x => x.Id);
                Random rand = new Random();
                int rId = rand.Next(1, maxId + 1);
                var check = _db.Pokemons.Find(rId);
                if (check == null) continue;
                randId = rId;
                break;
            }

            //инициализация строки для бд с покемоном дня
            row = new PokemonActual()
            {
                Value = "day",
                LastDayActual = today,
                PokemonId = randId
            };
            _db.PokemonActuals.Add(row);
            try
            {
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                BadRequest("Ошибка сохранения данных в бд");
            }
        }
        //уже есть запись, то есть покемон дня определялся ранее
        else
        {
            //если дата не актуальна
            if (today > row.LastDayActual)
            {
                //рандомное значение будет генерироваться пока не будет подходящим
                while (true)
                {
                    var maxId = await _db.Pokemons.MaxAsync(x => x.Id);
                    Random rand = new Random();
                    int rId = rand.Next(1, maxId + 1);
                    var check = _db.Pokemons.Find(rId);
                    if (check == null) continue;
                    randId = rId;
                    row.PokemonId = randId;
                    row.LastDayActual = today;
                    row.Id = 0;
                    _db.PokemonActuals.Add(row);
                    break;
                }

                try
                {
                    _db.SaveChanges();
                }
                catch (Exception e)
                {
                    BadRequest("Ошибка сохранения данных в бд");
                }
            }
        }

        int id = row.Id;
        row = _db.PokemonActuals.Include(x => x.Pokemon)
            .FirstOrDefault(c => c.Value == "day" && c.LastDayActual == today);

        return Ok(row.Pokemon.Title);
    }

    //  ПОЛУЧЕНИЕ ПОКЕМОНА НЕДЕЛИ ИСХОДЯ ИЗ РЕЙТИНГА
    [HttpGet]
    public async Task<IActionResult> GetPokemonWeek()
    {
        DateOnly startCurrentWeek;
        DateOnly endCurrentWeek;
        DateOnly startLastWeek;
        DateOnly endLastWeek;

        var today = DateOnly.FromDateTime(DateTime.Today);
        int indDayOfWeek = ((int)today.DayOfWeek - 1);

        //находим начало недели и конец
        startCurrentWeek = today.AddDays(-indDayOfWeek);
        endCurrentWeek = startCurrentWeek.AddDays(6);

        //находим информацию в бд о покемоне месяца
        var row = await _db.PokemonActuals.FirstOrDefaultAsync(x =>
            x.Value == "week" && x.LastDayActual == endCurrentWeek);


        //ранее не выбирался покемон недели
        if (row == null)
        {
            row = new PokemonActual()
            {
                LastDayActual = endCurrentWeek,
                Value = "week",
            };
        }
        else
        {
            if (row.PokemonId == null)
            {
                return Ok("Оценок не было");
            }

            endCurrentWeek = row.LastDayActual;
            startCurrentWeek = endCurrentWeek.AddDays(-6);
        }

        endLastWeek = startCurrentWeek.AddDays(-1);
        startLastWeek = endLastWeek.AddDays(-6);

        //неделя истекла или строка была не указана
        if (today > row.LastDayActual || row.PokemonId == null)
        {
            if (today > row.LastDayActual)
            {
                //определяем новые данные
                indDayOfWeek = ((int)today.DayOfWeek - 1);
                startCurrentWeek = today.AddDays(-indDayOfWeek);
                endCurrentWeek = startCurrentWeek.AddDays(6);
                endLastWeek = startCurrentWeek.AddDays(-1);
                startLastWeek = endLastWeek.AddDays(-6);
                row.LastDayActual = endCurrentWeek;
                row.PokemonId = null;
            }


            //берём все оценки за неделю
            var raitingByWeek =
                await _db.Raitings.Where(x => x.DataSet >= startLastWeek && x.DataSet <= endLastWeek).ToListAsync();
            if (raitingByWeek.Count == 0)
            {
                row.PokemonId = null;
                _db.PokemonActuals.Add(row);
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Ok(e);
                }

                return Ok("Оценок не было");
            }

            //Сортируем по убыванию по среднему значению звёзд и по количеству оценок покемона
            var pokemonRaiting = raitingByWeek.GroupBy(x => x.PokemonId).Select(p => new
            {
                PokemonId = p.Key,
                Count = p.Count(),
                ArgStar = (double)p.Sum(x => x.Star) / p.Count(),
            }).OrderByDescending(p => p.ArgStar).ThenByDescending(p => p.Count);

            //берём покемона с наибольшим рейтингом и наибольшим количеством оценок
            if (row.PokemonId == null)
            {
                row.PokemonId = pokemonRaiting.Select(x => x.PokemonId).FirstOrDefault();
                _db.PokemonActuals.Add(row);
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception e)
                {
                    return BadRequest(e);
                }
            }
        }


        row = _db.PokemonActuals.Include(x => x.Pokemon).FirstOrDefault(c => c.Value == "week");

        return Ok(row.Pokemon.Title);
    }

    //  ПОЛУЧЕНИЕ ПОКЕМОНА МЕСЯЦА ИСХОДЯ ИЗ РЕЙТИНГА
    [HttpGet]
    public async Task<IActionResult> GetPokemonMonth()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var row = _db.PokemonActuals.Where(x => x.Value == "month" && x.LastDayActual.Month == today.Month)
            .FirstOrDefault();

        DateOnly firstDayOfCurrentMonth;
        DateOnly lastDayOCurrentfMonth;

        //Покемон месяца не выбирался
        if (row == null)
        {
            firstDayOfCurrentMonth = new DateOnly(today.Year, today.Month, 1);
            lastDayOCurrentfMonth = firstDayOfCurrentMonth.AddMonths(1).AddDays(-1);
            row = new PokemonActual()
            {
                LastDayActual = lastDayOCurrentfMonth,
                Value = "month",
            };
        }
        else
        {
            if (row.PokemonId == null)
            {
                return Ok("Оценок не было");
            }
        }

        //если дата не актуальна или покемон не выбирался
        if (today.Month > row.LastDayActual.Month || row.PokemonId == null)
        {
            if (today.Month > row.LastDayActual.Month)
            {
                firstDayOfCurrentMonth = new DateOnly(today.Year, today.Month, 1);
                lastDayOCurrentfMonth = firstDayOfCurrentMonth.AddMonths(1).AddDays(-1);
                row.LastDayActual = lastDayOCurrentfMonth;
            }

            //берём все оценки за прошлый месяц
            var raitingByWeek =
                await _db.Raitings.Where(x => x.DataSet.Month == (today.Month - 1)).ToListAsync();
            if (raitingByWeek.Count == 0)
            {
                row.PokemonId = null;
                _db.PokemonActuals.Add(row);
                try
                {
                    _db.SaveChanges();
                }
                catch (Exception e)
                {
                    return Ok(e);
                }

                return Ok("Оценок не было");
            }

            //Сортируем по убыванию по среднему значению звёзд и по количеству оценок покемона
            var pokemonRaiting = raitingByWeek.GroupBy(x => x.PokemonId).Select(p => new
            {
                PokemonId = p.Key,
                Count = p.Count(),
                ArgStar = (double)p.Sum(x => x.Star) / p.Count(),
            }).OrderByDescending(p => p.ArgStar).ThenByDescending(p => p.Count);

            //берём покемона с наибольшим рейтингом и наибольшим количеством оценок
            if (row.PokemonId == null)
            {
                row.PokemonId = pokemonRaiting.Select(x => x.PokemonId).FirstOrDefault();
                _db.PokemonActuals.Add(row);

                try
                {
                    _db.SaveChanges();
                }
                catch (Exception e)
                {
                    return BadRequest(e);
                }
            }
        }

        row = _db.PokemonActuals.Include(x => x.Pokemon)
            .FirstOrDefault(c => c.Value == "month" && c.LastDayActual.Month == today.Month);
        return Ok(row.Pokemon.Title);
    }

//  ПОЛУЧЕНИЕ СПИСКА ВСЕХ ТИПОВ
    [HttpGet]
    public async Task<IActionResult> GetAllType()
    {
        return Ok(await _db.Types.OrderBy(x => x.Id).ToListAsync());
    }
}