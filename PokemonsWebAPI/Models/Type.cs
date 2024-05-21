using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PokemonsWebAPI.Models;

public partial class Type
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<PokemonType> PokemonTypes { get; } = new List<PokemonType>();
}
