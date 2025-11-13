using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using PromptGeneratorApp.Views;

namespace PromptGeneratorApp;

public partial class App : Application
{
    public App(MainPage mainPage)
    {
        InitializeComponent();

        var navigationPage = new NavigationPage(mainPage)
        {
            BarTextColor = Colors.White
        };

        if (Resources.TryGetValue("PrimaryColor", out var color) && color is Color primary)
        {
            navigationPage.BarBackgroundColor = primary;
        }

        MainPage = navigationPage;
    }
}
