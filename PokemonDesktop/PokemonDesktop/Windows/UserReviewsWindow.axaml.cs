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
using PokemonsDesktop.Models;

namespace PokemonDesktop.Windows;

public partial class UserReviewsWindow : Window
{
    private TextBlock _textBlockNoContent;
    private ListBox _listBoxRaiting;
    
    
    private int page = 1;
    private int count = 10;
    private int _userId;
    
    public UserReviewsWindow()
    {
        InitializeComponent();
        this.AttachDevTools();
    }
    
    public UserReviewsWindow(int userId)
    {
        InitializeComponent();
        this.AttachDevTools();

        SetReviewsListBox();
        _userId = userId;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        
        _listBoxRaiting = this.FindControl<ListBox>("ListBoxReviews");
        _textBlockNoContent = this.FindControl<TextBlock>("TextBlockNoContent");
    }

    private async void SetReviewsListBox()
    {
        await Task.Run(async () =>
        {
            string apiUrl = $"{DataManager.ApiHost}/User/GetRaitingByUser?userId={_userId}&page={page}&count={count}";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", DataManager.Token);
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            _textBlockNoContent.IsVisible = false;
                            _listBoxRaiting.IsVisible = true;
                        });

                        var responseValue = response.Content.ReadAsStringAsync().Result;
                        var list = JsonConvert.DeserializeObject<List<ReviewsListBoxModel>>(responseValue);
                        Dispatcher.UIThread.Post(() => { _listBoxRaiting.Items = list; });
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
                            _listBoxRaiting.IsVisible = false;
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
        });
    }

    private void ButtonPage_OnClick(object? sender, RoutedEventArgs e)
    {
        string bntName = (sender as Button).Name;


        switch (bntName)
        {
            case "ButtonLastPage":
                if (page > 0)
                {
                    --page;
                    SetReviewsListBox();
                }

                break;

            case "ButtonNextPage":
                ++page;
                SetReviewsListBox();
                break;
        }
    }

    private void Window_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            this.BeginMoveDrag(e);
        }
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

    private void ButtonClose_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}