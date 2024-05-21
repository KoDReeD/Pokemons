using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PokemonsWebAPI.Models;

public partial class UserActivity
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime Date { get; set; }

    public int ActivityId { get; set; }
    [JsonIgnore]
    public virtual Activity Activity { get; set; } = null!;
    [JsonIgnore]
    public virtual User UserNavigation { get; set; } = null!;
}
