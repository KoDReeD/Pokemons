using System;

namespace PokemonsDesktop.Models;

public class ReviewsListBoxModel
{
    public string Title { get; set; }
    public int Star { get; set; }
    public DateOnly DateSet { get; set; }
    public string Username { get; set; }
}