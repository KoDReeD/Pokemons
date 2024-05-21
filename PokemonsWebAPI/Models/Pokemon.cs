using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PokemonsWebAPI.Models;

public partial class Pokemon
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string PhotoPath { get; set; } = null!;

    public int ParameterId { get; set; }

    public int HarakteristikId { get; set; }
    [JsonIgnore]
    public virtual PokemonHarakteristik Harakteristik { get; set; } = null!;
    [JsonIgnore]
    public virtual PokemonParameter Parameter { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<PokemonAbility> PokemonAbilities { get; } = new List<PokemonAbility>();
    [JsonIgnore]
    public virtual ICollection<PokemonActual> PokemonActuals { get; } = new List<PokemonActual>();
    [JsonIgnore]
    public virtual ICollection<PokemonType> PokemonTypes { get; } = new List<PokemonType>();
    [JsonIgnore]
    public virtual ICollection<Raiting> Raitings { get; } = new List<Raiting>();
}
