using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

public partial class AddUserWindow : Window
{
    private UserVM _user = new UserVM();

    private ComboBox _comboBoxRoles;
    private TextBox _textBoxUsername;
    private TextBox _textBoxPassword;

    public AddUserWindow()
    {
        InitializeComponent();
        this.AttachDevTools();

        this.DataContext = _user;
        
        SetComboRoles();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        _comboBoxRoles = this.FindControl<ComboBox>("ComboBoxRoles");
    }

    private async void ButtonAddUser_OnClick(object? sender, RoutedEventArgs e)
    {
        StringBuilder errors = new StringBuilder();

        if (string.IsNullOrWhiteSpace(_user.Username))
        {
            errors.AppendLine("Укажите имя пользователя");
        }

        if (string.IsNullOrWhiteSpace(_user.Password))
        {
            errors.AppendLine("Укажите пароль");
        }

        if (_user.SelectedRole == null)
        {
            errors.AppendLine("Укажите роль");
        }

        if (errors.Length > 0)
        {
            var messageBox = await MyMessageBox.CreateDialog(this, MyMessageBox.MessageBoxButtons.Ok, "Заполните поля",
                errors.ToString());
            return;
        }
        
        _user.Username = _user.Username.Replace("\n", "");
        _user.Username = _user.Username.Replace("\r", "");

        string apiUrl = $"{DataManager.ApiHost}/User/AddUser?login={_user.Username}&pass={_user.Password}&roleId={_user.SelectedRole.Id}";

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", DataManager.Token);
            HttpResponseMessage response = await client.PostAsync(apiUrl, null);
            try
            {
                string message = GetResponseText(response).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = await MyMessageBox.CreateDialog(this, MyMessageBox.MessageBoxButtons.Ok, "Успех",
                        "Пользователь добавлен");
                    this.Close();
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
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var result = await MyMessageBox.CreateDialog(this, MyMessageBox.MessageBoxButtons.Ok, "Ошибка",
                        $"{message}");
                }
            }
            //  если нет интернета
            catch (Exception ex)
            {
                SetComboRoles();
            }
        }
    }

    private async void SetComboRoles()
    {
        string apiUrl = $"{DataManager.ApiHost}/User/GetAllRoles";

        await Task.Run(async () =>
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", DataManager.Token);
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                try
                {
                    var responseValue = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var list = JsonConvert.DeserializeObject<List<UserRole>>(responseValue);

                        Dispatcher.UIThread.Post(() =>
                        {
                            _comboBoxRoles.Items = list;
                            _comboBoxRoles.SelectedIndex = 2;
                        });
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
                    else
                    {
                        SetComboRoles();
                    }
                }
                //  если нет интернета
                catch (Exception ex)
                {
                    SetComboRoles();
                }
            }
        });
    }
    
    public async Task<string> GetResponseText(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return text;
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
}