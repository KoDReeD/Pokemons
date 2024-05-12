using Avalonia.Media.Imaging;
using Newtonsoft.Json;

namespace PokemonsDesktop.Models;

public class PokemonVM
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string PhotoPath { get; set; }
    
    [JsonIgnore] 
    public Bitmap Image { get; set; }
}