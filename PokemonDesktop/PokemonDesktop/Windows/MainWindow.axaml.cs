using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Newtonsoft.Json;
using PokemonDesktop.Data;
using PokemonDesktop.Pages;
using PokemonsDesktop.Models;

namespace PokemonDesktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataManager.MainWindow = this;
        contentControl.Content = new PokemonsPages();
    }

    private void MenuButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var btn = sender as Button;

        switch (btn.Name)
        {
            case "ButtonPokemon":
                contentControl.Content = new PokemonsPages();
                break;

            case "ButtonRaiting":
                contentControl.Content = new RaitingPage();
                break;

            case "ButtonUsers":
                contentControl.Content = new UsersPage();
                break;
                
            case "ButtonLikes":
                contentControl.Content = new LikesPage();
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

    private async void Window_OnClosing(object? sender, CancelEventArgs e)
    {
        string apiUrl = $"{DataManager.ApiHost}/Account/SignOut";
                    
        var jsonActivity = new
        {
            UserId = DataManager.UserId,
            DateTime = DateTime.Now
        };

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, jsonActivity);
            try
            {
                var responseValueStr = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = await MyMessageBox.CreateDialog(this, MyMessageBox.MessageBoxButtons.Ok,
                        "Ошибка", "Sign out");
                }

            }
            catch (Exception ex)
            {
                var message = await MyMessageBox.CreateDialog(this, MyMessageBox.MessageBoxButtons.Ok,
                    "Ошибка", "Sign out");
            }
        }
        
    }
}