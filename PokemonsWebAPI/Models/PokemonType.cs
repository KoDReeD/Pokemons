using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PokemonsWebAPI.Models;

public partial class PokemonType
{
    public int Id { get; set; }

    public int TypeId { get; set; }

    public int PokemonId { get; set; }
    [JsonIgnore]
    public virtual Pokemon Pokemon { get; set; } = null!;
    [JsonIgnore]
    public virtual Type Type { get; set; } = null!;
}
