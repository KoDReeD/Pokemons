using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Newtonsoft.Json;
using PokemonDesktop.Data;
using PokemonsDesktop.Models;
using Avalonia.Media.Imaging;
using PokemonDesktop.Windows;
using SkiaSharp;

namespace PokemonDesktop.Pages;

public partial class PokemonsPages : UserControl
{
    private ListBox _listBoxPokemons;
    private TextBlock _textBlockNoContent;
    private Button _buttonSort;
    private TextBox _textBoxFound;

    private Button _buttonLastPAge;
    private Button _buttonNextPage;

    private int page = 1;
    private int count = 20;

    public PokemonsPages()
    {
        InitializeComponent();

        Init();
    }

    public async void Init()
    {
        await SetPokemonsListBox();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        _listBoxPokemons = this.FindControl<ListBox>("PokemonsListBox");
        _textBlockNoContent = this.FindControl<TextBlock>("TextBlockNoContent");
        _buttonSort = this.FindControl<Button>("ButtonOrderByTitle");
        _textBoxFound = this.FindControl<TextBox>("TextBoxFound");

        _buttonLastPAge = this.FindControl<Button>("ButtonLastPage");
        _buttonNextPage = this.FindControl<Button>("ButtonNextPage");
    }

    private async Task SetPokemonsListBox()
    {
        string title = _textBoxFound.Text;
        bool sordByASC = true;

        if (_buttonSort.Tag.ToString() == "DESC")
        {
            sordByASC = false;
        }

        string apiUrl = $"{DataManager.ApiHost}/Pokemon";

        if (!string.IsNullOrWhiteSpace(title))
        {
            apiUrl += $"/GetAllByPageAndTitle?page={page}&count={count}&sortByASC={sordByASC}&title={title}";
        }
        else
        {
            apiUrl += $"/GetAllByPage?page={page}&count={count}&sortByASC={sordByASC}";
        }

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", DataManager.Token);
            HttpResponseMessage response = await client.GetAsync(apiUrl);
            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseValue = response.Content.ReadAsStringAsync().Result;
                    var list = JsonConvert.DeserializeObject<List<PokemonVM>>(responseValue);
                    _textBlockNoContent.IsVisible = false;
                    _listBoxPokemons.IsVisible = true;

                    foreach (var obj in list)
                    {
                        using (var webClient = new WebClient())
                        {
                            byte[] imageData = await webClient.DownloadDataTaskAsync(obj.PhotoPath);
                            using (var stream = new MemoryStream(imageData))
                            {
                                obj.Image = new Bitmap(stream);
                            }
                        }
                    }

                    _listBoxPokemons.Items = list;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    try
                    {
                        AuthorizeWindow authorizeWindow = new AuthorizeWindow();
                        authorizeWindow.Show();
                        DataManager.MainWindow.Close();
                    }
                    catch
                    {
                    }
                }
                else if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    _listBoxPokemons.IsVisible = false;
                    _textBlockNoContent.IsVisible = true;
                }
                else if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    if (page > 1)
                    {
                        --page;
                    }
                }
            }
            //  если нет интернета
            catch (Exception ex)
            {
                if (page > 1)
                {
                    --page;
                }
            }
        }

        _buttonLastPAge.IsVisible = true;
        _buttonNextPage.IsVisible = true;
    }

    private async void ButtonOrderByTitle_OnClick(object? sender, RoutedEventArgs e)
    {
        Button btn = sender as Button;

        switch (btn.Tag.ToString())
        {
            case "ASC":
                btn.Tag = "DESC";
                await SetPokemonsListBox();
                break;

            case "DESC":
                btn.Tag = "ASC";
                await SetPokemonsListBox();
                break;
        }
    }


    private async void ButtonPage_OnClick(object? sender, RoutedEventArgs e)
    {
        string bntName = (sender as Button).Name;

        switch (bntName)
        {
            case "ButtonLastPage":
                if (page > 1)
                {
                    --page;
                    _buttonLastPAge.IsVisible = false;
                    _buttonNextPage.IsVisible = false;
                    await SetPokemonsListBox();
                }

                break;

            case "ButtonNextPage":
                page++;
                _buttonLastPAge.IsVisible = false;
                _buttonNextPage.IsVisible = false;
                await SetPokemonsListBox();
                break;
        }
    }

    private async void TextBoxFound_OnKeyUp(object? sender, KeyEventArgs e)
    {
        page = 1;

        await SetPokemonsListBox();
    }

    private void ButtonDetails_OnClick(object? sender, RoutedEventArgs e)
    {
        Button btn = sender as Button;

        int id = int.Parse(btn.Tag.ToString());

        PokemonDetailsInfoWindow pokemonDetailsInfoWindow = new PokemonDetailsInfoWindow(id);
        pokemonDetailsInfoWindow.ShowDialog(DataManager.MainWindow);
    }
}