using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PokemonDesktop.Windows;

public partial class ChangePassword : Window
{
    public ChangePassword()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}