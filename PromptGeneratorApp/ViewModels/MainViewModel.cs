using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using PromptGeneratorApp.Data;
using PromptGeneratorApp.Models;

namespace PromptGeneratorApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly PromptRepository _repository;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string context = string.Empty;

    [ObservableProperty]
    private string selectedLanguage = "C#";

    public ObservableCollection<Prompt> Prompts { get; } = new();

    public IReadOnlyList<string> Languages { get; } = new[]
    {
        ".NET MAUI",
        "XAML",
        "C#",
        "SQLite"
    };

    public MainViewModel(PromptRepository repository)
    {
        _repository = repository;
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        Prompts.Clear();
        var prompts = await _repository.GetPromptsAsync();
        foreach (var prompt in prompts)
        {
            Prompts.Add(prompt);
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Context))
        {
            if (Application.Current?.MainPage is not null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Datos incompletos",
                    "Completa título y contexto para generar un prompt.",
                    "Entendido");
            }

            return;
        }

        var prompt = new Prompt(Title, Context, SelectedLanguage);
        await _repository.AddPromptAsync(prompt);
        Prompts.Add(prompt);

        Title = string.Empty;
        Context = string.Empty;
    }
}
