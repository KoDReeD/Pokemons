using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PokemonsWebAPI.Models;

public partial class PokemonHarakteristik
{
    public int Id { get; set; }

    public int Healt { get; set; }

    public int Attack { get; set; }

    public int Protect { get; set; }

    public int SpecAttack { get; set; }

    public int SpecProtect { get; set; }

    public int Speed { get; set; }

    public int? Sum { get; set; }
    [JsonIgnore]
    public virtual ICollection<Pokemon> Pokemons { get; } = new List<Pokemon>();
}
