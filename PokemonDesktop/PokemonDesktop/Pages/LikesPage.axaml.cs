using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Newtonsoft.Json;
using PokemonDesktop.Data;
using PokemonDesktop.Windows;
using PokemonsDesktop.Models;

namespace PokemonDesktop.Pages;

public partial class LikesPage : UserControl
{
    private ListBox _listBoxLikes;
    private TextBlock _textBlockNoContent;
    private Button _buttonSort;
    private TextBox _textBoxFound;
    
    private Button _buttonLastPAge;
    private Button _buttonNextPage;

    private int page = 1;
    private int count = 10;

    public LikesPage()
    {
        InitializeComponent();
        
        SetLikesListBox();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        _listBoxLikes = this.FindControl<ListBox>("ListBoxLikes");
        _textBlockNoContent = this.FindControl<TextBlock>("TextBlockNoContent");
        _buttonSort = this.FindControl<Button>("ButtonOrderByDate");
        _textBoxFound = this.FindControl<TextBox>("TextBoxFound");
        
        _buttonLastPAge = this.FindControl<Button>("ButtonLastPage");
        _buttonNextPage = this.FindControl<Button>("ButtonNextPage");
    }

    private async void SetLikesListBox()
    {
        string text = _textBoxFound.Text;
        
        bool orderBy = true;

        if (_buttonSort.Tag.ToString() == "DESC")
        {
            orderBy = false;
        }

        string apiUrl = $"{DataManager.ApiHost}/Like";
        
        if (!string.IsNullOrWhiteSpace(text))
        {
            apiUrl += $"/GetAllPageByTitle?page={page}&count={count}&orderByASC={orderBy}&text={text}";
        }
        else
        {
            apiUrl += $"/GetAllByPage?page={page}&count={count}&orderByASC={orderBy}";
        }

        await Task.Run(async () =>
        {
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
                        var list = JsonConvert.DeserializeObject<List<VMPokemonRaiting>>(responseValue);
                        Dispatcher.UIThread.Post(() =>
                        {
                            _textBlockNoContent.IsVisible = false;
                            _listBoxLikes.IsVisible = true;
                            _listBoxLikes.Items = list;
                        });
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        if (page > 1)
                        {
                            --page;
                        }
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
                        Dispatcher.UIThread.Post(() =>
                        {
                            _listBoxLikes.IsVisible = false;
                            _textBlockNoContent.IsVisible = true;
                        });
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
            Dispatcher.UIThread.Post(() =>
            {
                _buttonLastPAge.IsVisible = true;
                _buttonNextPage.IsVisible = true;
            });
        });
    }

    private void ButtonPage_OnClick(object? sender, RoutedEventArgs e)
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
                    SetLikesListBox();
                }

                break;

            case "ButtonNextPage":
                page++;
                _buttonLastPAge.IsVisible = false;
                _buttonNextPage.IsVisible = false;
                SetLikesListBox();
                break;
        }
    }

    private void ButtonOrderByDate_OnClick(object? sender, RoutedEventArgs e)
    {
        Button btn = sender as Button;

        switch (btn.Tag.ToString())
        {
            case "ASC":
                btn.Tag = "DESC";
                SetLikesListBox();
                break;

            case "DESC":
                btn.Tag = "ASC";
                SetLikesListBox();
                break;
        }
    }
    

    private void TextBoxFound_OnKeyUp(object? sender, KeyEventArgs e)
    {
        page = 1;
        SetLikesListBox();
    }
}