using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PokemonsWebAPI.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public bool PasswordChange { get; set; }
    [JsonIgnore]
    public virtual ICollection<Raiting> Raitings { get; } = new List<Raiting>();
    [JsonIgnore]
    public virtual UserRole Role { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<UserActivity> UserActivities { get; } = new List<UserActivity>();
}
