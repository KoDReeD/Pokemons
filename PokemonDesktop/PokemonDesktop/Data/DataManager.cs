using Avalonia.Controls;

namespace PokemonDesktop.Data;

public static class DataManager
{
    public static Window MainWindow { get; set; }

    public static string ApiHost { get; set; } = "https://kodred.bsite.net";

    public static int UserId { get; set; }
    
    public static string Token { get; set; }
}