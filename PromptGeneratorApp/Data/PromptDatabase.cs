using System.IO;
using Microsoft.Maui.Storage;
using SQLite;

namespace PromptGeneratorApp.Data;

public class PromptDatabase
{
    private const string DatabaseFileName = "prompts.db3";
    private static readonly string DatabasePath =
        Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);

    private readonly SQLiteAsyncConnection _database;

    public PromptDatabase()
    {
        _database = new SQLiteAsyncConnection(DatabasePath);
    }

    public async Task InitializeAsync()
    {
        await _database.CreateTableAsync<PromptRecord>();
    }

    public Task<List<PromptRecord>> GetPromptsAsync() => _database.Table<PromptRecord>().ToListAsync();

    public Task<int> SavePromptAsync(PromptRecord record) => _database.InsertAsync(record);
}

public class PromptRecord
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(120)]
    public string Title { get; set; } = string.Empty;

    public string Context { get; set; } = string.Empty;

    public string Language { get; set; } = string.Empty;
}
