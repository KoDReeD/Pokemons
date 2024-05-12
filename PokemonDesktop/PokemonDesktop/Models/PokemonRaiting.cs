using System;

namespace PokemonsDesktop.Models;

public class VMPokemonRaiting
{
    public int Id { get; set; }
    public string Pokemon { get; set; }
    public int Star { get; set; }
    public DateOnly DataSet { get; set; }
    public string Username { get; set; }
}