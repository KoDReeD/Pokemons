using System;
using System.Collections.Generic;
using System.Diagnostics;
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

public partial class UserActivityWindow : Window
{
    private TextBlock _textBlockNoContent;
    private ListBox _listBoxUserActivity;
    
    
    private int page = 1;
    private int count = 10;
    private int _userId;
    
    public UserActivityWindow()
    {
        InitializeComponent();
        this.AttachDevTools();
    }
    
    public UserActivityWindow(int userId)
    {
        InitializeComponent();
        this.AttachDevTools();

        SetActivityListBox();
        _userId = userId;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        
        _listBoxUserActivity = this.FindControl<ListBox>("ListBoxUserActivity");
        _textBlockNoContent = this.FindControl<TextBlock>("TextBlockNoContent");
    }

    private async void SetActivityListBox()
    {
        await Task.Run(async () =>
        {
            string apiUrl = $"{DataManager.ApiHost}/Account/GetActivityByUserId?page={page}&count={count}&userId={_userId}";

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
                            _listBoxUserActivity.IsVisible = true;
                        });

                        var responseValue = response.Content.ReadAsStringAsync().Result;
                        var list = JsonConvert.DeserializeObject<List<UserActivity>>(responseValue);
                        Dispatcher.UIThread.Post(() => { _listBoxUserActivity.Items = list; });
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
                            _listBoxUserActivity.IsVisible = false;
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
                    SetActivityListBox();
                }

                break;

            case "ButtonNextPage":
                ++page;
                SetActivityListBox();
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