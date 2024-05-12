using System;
using System.Collections.Generic;
using System.Linq;
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

public partial class UsersPage : UserControl
{
    private ListBox _listBoxUsers;
    private TextBox _textBoxFoundTitle;
    private Button _buttonOrderByUsername;
    private TextBlock _textBlockNoContent;

    private Button _buttonLastPAge;
    private Button _buttonNextPage;


    private int page = 1;
    private int count = 10;


    public UsersPage()
    {
        InitializeComponent();
        SetUserListBox();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        _listBoxUsers = this.FindControl<ListBox>("UserListBox");
        _textBoxFoundTitle = this.FindControl<TextBox>("TextBoxFoundTitle");
        _buttonOrderByUsername = this.FindControl<Button>("ButtonOrderByUsername");
        _textBlockNoContent = this.FindControl<TextBlock>("TextBlockNoContent");

        _buttonLastPAge = this.FindControl<Button>("ButtonLastPage");
        _buttonNextPage = this.FindControl<Button>("ButtonNextPage");
    }


    private async void SetUserListBox()
    {
        string apiUrl = $"{DataManager.ApiHost}/User";
        string title = _textBoxFoundTitle.Text;
        bool sordByASC = true;
        
        if (_buttonOrderByUsername.Tag.ToString() == "DESC")
        {
            sordByASC = false;
        }

        if (!string.IsNullOrWhiteSpace(title))
        {
            apiUrl += $"/GetAllByPageByUsername?page={page}&count={count}&sortByASC={sordByASC}&title={title}";
        }
        else
        {
            //https://workhost.bsite.net/Category
            apiUrl += $"/GetAllByPage?page={page}&count={count}&sortByASC={sordByASC}";
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
                    var responseValue = response.Content.ReadAsStringAsync().Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            _textBlockNoContent.IsVisible = false;
                            _listBoxUsers.IsVisible = true;
                        });

                        responseValue = response.Content.ReadAsStringAsync().Result;
                        var list = JsonConvert.DeserializeObject<List<UserListBoxModel>>(responseValue);
                        Dispatcher.UIThread.Post(() =>
                        {
                            _listBoxUsers.Items = list.Select(x => new
                            {
                                Id = x.Id,
                                Username = x.Username,
                                Password = x.Password,
                                RoleName = x.RoleName,
                                ChangePassword = x.ChangePassword,
                                IsAdmin = x.RoleId == 1 ? false : true
                            });
                        });
                    }
                    else if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            _listBoxUsers.IsVisible = false;
                            _textBlockNoContent.IsVisible = true;
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
                _buttonNextPage.IsVisible = false;
            });
        });
    }

    private void ButtonOrderByTitle_OnClick(object? sender, RoutedEventArgs e)
    {
        Button btn = sender as Button;

        switch (btn.Tag.ToString())
        {
            case "ASC":
                btn.Tag = "DESC";
                SetUserListBox();
                break;

            case "DESC":
                btn.Tag = "ASC";
                SetUserListBox();
                break;
        }
    }

    private void ButtonAddEdit_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private async void ButtonPage_OnClick(object? sender, RoutedEventArgs e)
    {
        string bntName = (sender as Button).Name;

        await Task.Run(async () =>
        {
            switch (bntName)
            {
                case "ButtonLastPage":
                    if (page > 1)
                    {
                        --page;
                        _buttonLastPAge.IsVisible = false;
                        _buttonNextPage.IsVisible = false;
                        SetUserListBox();
                    }

                    break;

                case "ButtonNextPage":
                    page++;
                    _buttonLastPAge.IsVisible = false;
                    _buttonNextPage.IsVisible = false;
                    SetUserListBox();

                    break;
            }
        });
    }


    private void InputElement_OnKeyUp(object? sender, KeyEventArgs e)
    {
        page = 1;

        SetUserListBox();
    }

    private void ButtonUserAdd_OnClick(object? sender, RoutedEventArgs e)
    {
        AddUserWindow addUserWindow = new AddUserWindow();
        addUserWindow.Closed += (sender, e) => { SetUserListBox(); };
        addUserWindow.ShowDialog(DataManager.MainWindow);
    }

    private async void ButtonResetPass_OnClick(object? sender, RoutedEventArgs e)
    {
        Button btn = sender as Button;
        int userId = int.Parse(btn.Tag.ToString());

        var result = await MyMessageBox.CreateDialog(DataManager.MainWindow, MyMessageBox.MessageBoxButtons.YesNo,
            "Подтвердить действие", "Вы уверены, что ходить сбросить пароль этому пользователю?");
        if (result != MyMessageBox.MessageBoxResult.Yes)
        {
            return;
        }
        else
        {
            string apiUrl = $"{DataManager.ApiHost}/Account/PasswordReset?userId={userId}";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync(apiUrl, null);
                try
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var res = await MyMessageBox.CreateDialog(DataManager.MainWindow,
                            MyMessageBox.MessageBoxButtons.Ok, "Успех", "Пароль сброшен");

                        SetUserListBox();
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        if (page > 1)
                        {
                            --page;
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        if (page > 1)
                        {
                            --page;
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                        var res = await MyMessageBox.CreateDialog(DataManager.MainWindow,
                            MyMessageBox.MessageBoxButtons.Ok, "Ошибка", "У пользователя уже сброшен пароль");
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
        }
    }

    private void ButtonActivity_OnClick(object? sender, RoutedEventArgs e)
    {
        Button btn = sender as Button;
        int userId = int.Parse(btn.Tag.ToString());

        UserActivityWindow userActivityWindow = new UserActivityWindow(userId);
        userActivityWindow.ShowDialog(DataManager.MainWindow);
    }

    private void ButtonReviews_OnClick(object? sender, RoutedEventArgs e)
    {
        Button btn = sender as Button;
        int userId = int.Parse(btn.Tag.ToString());

        UserReviewsWindow userReviewsWindow = new UserReviewsWindow(userId);
        userReviewsWindow.ShowDialog(DataManager.MainWindow);
    }
}