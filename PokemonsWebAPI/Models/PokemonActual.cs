using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PokemonsWebAPI.Models;

public partial class PokemonActual
{
    public int Id { get; set; }

    public string Value { get; set; } = null!;

    public DateOnly LastDayActual { get; set; }

    public int? PokemonId { get; set; }
    [JsonIgnore]
    public virtual Pokemon? Pokemon { get; set; }
}
