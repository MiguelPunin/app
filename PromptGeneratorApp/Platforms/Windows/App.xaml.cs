namespace PromptGeneratorApp.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        InitializeComponent();
    }

    protected override MauiApp CreateMauiApp() => PromptGeneratorApp.MauiProgram.CreateMauiApp();
}
