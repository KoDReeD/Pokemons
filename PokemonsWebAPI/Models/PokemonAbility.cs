using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PokemonsWebAPI.Models;

public partial class PokemonAbility
{
    public int Id { get; set; }

    public int AbilityId { get; set; }

    public int PokemonId { get; set; }
    [JsonIgnore]
    public virtual Ability Ability { get; set; } = null!;
    [JsonIgnore]
    public virtual Pokemon Pokemon { get; set; } = null!;
}
