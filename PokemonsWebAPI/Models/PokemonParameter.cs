using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PokemonsWebAPI.Models;

public partial class PokemonParameter
{
    public int Id { get; set; }

    public decimal Height { get; set; }

    public decimal Weigh { get; set; }

    public int AllInGame { get; set; }

    public int Shainy { get; set; }

    public int Rarity { get; set; }

    public int? HatchingTime { get; set; }

    public int ExpGroup { get; set; }

    public int EvolutionStage { get; set; }
    [JsonIgnore]
    public virtual Group ExpGroupNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<Pokemon> Pokemons { get; } = new List<Pokemon>();
}
