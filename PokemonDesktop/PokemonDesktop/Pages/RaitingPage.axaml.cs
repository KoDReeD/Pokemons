using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using PokemonDesktop.Data;
using PokemonDesktop.Windows;

namespace PokemonDesktop.Pages;

public partial class RaitingPage : UserControl
{
    
    private TextBlock _textBlockPokemonDay;
    private TextBlock _textBlockPokemonWeek;
    private TextBlock _textBlockPokemonMonth;

    public RaitingPage()
    {
        InitializeComponent();
        
        SetPokemonsRaiting();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        
        _textBlockPokemonDay = this.FindControl<TextBlock>("TextBlockPokDay");
        _textBlockPokemonWeek = this.FindControl<TextBlock>("TextBlockPokWeek");
        _textBlockPokemonMonth = this.FindControl<TextBlock>("TextBlockPokMonth");
    }
    
    private async void SetPokemonsRaiting()
    {
        await Task.Run(async () =>
        {

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", DataManager.Token);
            string baseUrlPokDay = $"{DataManager.ApiHost}/Pokemon/GetPokemonDay";
            
            HttpClient client2 = new HttpClient();
            client2.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", DataManager.Token);
            string baseUrlPokWeek = $"{DataManager.ApiHost}/Pokemon/GetPokemonWeek";
            
            HttpClient client3 = new HttpClient();
            client3.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", DataManager.Token);
            string baseUrlPokMonth = $"{DataManager.ApiHost}/Pokemon/GetPokemonMonth";
            
            var pokDay = await client.GetAsync(baseUrlPokDay);
            var pokWeek = await client2.GetAsync(baseUrlPokWeek);
            var pokMonth = await client3.GetAsync(baseUrlPokMonth);
            
            try
            {
                if (pokDay.StatusCode == HttpStatusCode.OK)
                {
                    var responseValue = pokDay.Content.ReadAsStringAsync().Result;

                    Dispatcher.UIThread.Post(() =>
                    {
                        _textBlockPokemonDay.Text = responseValue;
                    });
                }
                else if (pokDay.StatusCode == HttpStatusCode.Unauthorized)
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

                if (pokWeek.StatusCode == HttpStatusCode.OK)
                {
                    var responseValue = pokWeek.Content.ReadAsStringAsync().Result;
                    
                    Dispatcher.UIThread.Post(() =>
                    {
                        _textBlockPokemonWeek.Text = responseValue;
                    });
                }
                else if (pokWeek.StatusCode == HttpStatusCode.Unauthorized)
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

                if (pokMonth.StatusCode == HttpStatusCode.OK)
                {
                    var responseValue = pokMonth.Content.ReadAsStringAsync().Result;
                    
                    Dispatcher.UIThread.Post(() =>
                    {
                        _textBlockPokemonMonth.Text = responseValue;
                    });
                }
                else if (pokMonth.StatusCode == HttpStatusCode.Unauthorized)
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        });
        
        // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserDataManager.Token);
        // client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserDataManager.Token);
        // client3.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserDataManager.Token);
        
    }
}