using System;

namespace PokemonsDesktop.Models;

public class UserActivity
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Activity { get; set; }
    public DateTime Date { get; set; }
}