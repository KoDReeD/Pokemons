using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Newtonsoft.Json;
using PokemonDesktop.Data;
using PokemonsWebAPI.Models;

namespace PokemonDesktop.Windows;

public partial class AuthorizeWindow : Window
{
    private TextBox _textBoxPass;
    private TextBox _textBoxUsername;
    private Image ImageHidePass;
    
    
    public AuthorizeWindow()
    {
        InitializeComponent();
        this.AttachDevTools();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        
        _textBoxPass = this.FindControl<TextBox>("TextBoxPassword");
        ImageHidePass = this.FindControl<Image>("ImagePassHide");
        _textBoxUsername = this.FindControl<TextBox>("TextBoxUsername");
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

    private void ButtonHidePassword_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_textBoxPass.PasswordChar == '\0')
        {
            string path = @"..\..\..\Assets\Icons\eye.png";
            ImageHidePass.Source = new Bitmap(path);
            _textBoxPass.PasswordChar = '*';
        }
        else
        {
            string path = @"..\..\..\Assets\Icons\hide_eye.png";
            ImageHidePass.Source = new Bitmap(path);
            _textBoxPass.PasswordChar = '\0';
        }
    }

    private async void ButtonLogIn_OnClick(object? sender, RoutedEventArgs e)
    {
        Button btn = sender as Button;
        btn.IsEnabled = false;
        
        string pass = _textBoxPass.Text;
        string username = _textBoxUsername.Text;

        if (string.IsNullOrWhiteSpace(pass) || string.IsNullOrWhiteSpace(username))
        {
            var message = await MyMessageBox.CreateDialog(this, MyMessageBox.MessageBoxButtons.Ok,
                "Ошибка", "Заполните все поля");
            btn.IsEnabled = true;
            return;
        }

        var json = new
        {
            Username = username,
            Password = pass
        };
        
        using (HttpClient httpClient = new HttpClient())
        {
            string apiUrl = $"{DataManager.ApiHost}/Account/Authorization";
            HttpResponseMessage response = await httpClient.PostAsJsonAsync(apiUrl, json);
            try
            {
                var responseValue = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseValue);

                    DataManager.UserId = authResponse.UserId;
                    DataManager.Token = authResponse.Token;
                    
                    if (authResponse.RoleId != 1)
                    {
                        var message = await MyMessageBox.CreateDialog(this, MyMessageBox.MessageBoxButtons.Ok,
                            "Ошибка доступа", "Доступ запрещён вы не администратор");
                        btn.IsEnabled = true;
                        return;
                    }

                    apiUrl = $"{DataManager.ApiHost}/Account/SignIn";
                    
                    var jsonActivity = new
                    {
                        UserId = DataManager.UserId,
                        DateTimeOffset.Now
                    };
                    
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", DataManager.Token);
                    
                    HttpResponseMessage responseBody = await httpClient.PostAsJsonAsync(apiUrl, jsonActivity);
                    try
                    {
                        if (responseBody.StatusCode != HttpStatusCode.OK)
                        {
                            var s = await responseBody.Content.ReadAsStringAsync();
                            var message = await MyMessageBox.CreateDialog(this, MyMessageBox.MessageBoxButtons.Ok,
                                "Ошибка", "Sign in");
                            btn.IsEnabled = true;
                            return;
                        }
                    
                    }
                    catch (Exception ex)
                    {
                        var message = await MyMessageBox.CreateDialog(this, MyMessageBox.MessageBoxButtons.Ok,
                            "Ошибка", "Sign in");
                        btn.IsEnabled = true;
                        return;
                    }

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var message = await MyMessageBox.CreateDialog(this, MyMessageBox.MessageBoxButtons.Ok,
                        "Ошибка", responseValue);
                    btn.IsEnabled = true;
                    return;
                }
                
                else if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    if (responseValue != "1")
                    {
                        var message = await MyMessageBox.CreateDialog(this, MyMessageBox.MessageBoxButtons.Ok,
                            "Ошибка доступа", "Доступ запрещён вы не администратор");
                        btn.IsEnabled = true;
                        return;
                    }
                    else
                    {
                        
                    }
                }
            }
            catch (Exception ex)
            {
                btn.IsEnabled = true;
            }
        }
    }
}