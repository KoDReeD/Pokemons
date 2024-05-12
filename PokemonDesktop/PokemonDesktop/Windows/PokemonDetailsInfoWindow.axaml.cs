using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoMapper;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Newtonsoft.Json;
using PokemonDesktop.Data;

namespace PokemonDesktop.Windows;

public class PokemonDetails
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string PhotoPath { get; set; }
    public int Attack { get; set; }
    public int Healt { get; set; }
    public int Speed { get; set; }
    public int Protect { get; set; }
    public int Sum { get; set; }
    public int SpecAttack { get; set; }
    public int SpecProtect { get; set; }
    public decimal Height { get; set; }
    public decimal Weigh { get; set; }
    public string Group { get; set; }
    public int Rarity { get; set; }
    public int Shainy { get; set; }
    public int? HatchingTime { get; set; }
    public int AllInGame { get; set; }
    public int EvolutionStage { get; set; }
    
    [System.Text.Json.Serialization.JsonIgnore]
    public Bitmap Photo { get; set; }
}

public partial class PokemonDetailsInfoWindow : Window
{
    private int id { get; set; }
    private PokemonDetails currentPokemon = new PokemonDetails();
    public PokemonDetailsInfoWindow()
    {
        InitializeComponent();
        this.AttachDevTools();
    }
    
    public PokemonDetailsInfoWindow(int id)
    {
        InitializeComponent();
        this.AttachDevTools();

        this.id = id;
        
        SetContext();
    }

    public async void SetContext()
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", DataManager.Token);
            string url = $"{DataManager.ApiHost}/Pokemon/FoundPokemonFullInfo?id={id}";

            var response = await client.GetAsync(url);

            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var fullInfo = JsonConvert.DeserializeObject<PokemonDetails>(json);

                    using (var webClient = new WebClient())
                    {
                        byte[] imageData = webClient.DownloadData(fullInfo.PhotoPath);
                        using (var stream = new MemoryStream(imageData))
                        {
                            fullInfo.Photo = new Bitmap(stream);
                        }
                    }

                    DataContext = fullInfo;
                }
                else
                {
                    
                }
            }
            catch (Exception e)
            {
                
            }
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }


    private void Window_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            this.BeginMoveDrag(e);
        }
    }

    private void ButtonClose_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ButtonWinSize_OnClick(object? sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Normal)
        {
            WindowState = WindowState.Maximized;
        }
        else
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
        }
    }
}