using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PokemonsWebAPI.Models;

public partial class Raiting
{
    public int Id { get; set; }

    public int PokemonId { get; set; }

    public int UserId { get; set; }

    public int Star { get; set; }

    public DateOnly DataSet { get; set; }
    [JsonIgnore]
    public virtual Pokemon Pokemon { get; set; } = null!;
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
