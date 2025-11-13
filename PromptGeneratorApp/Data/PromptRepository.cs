using System.Collections.Generic;
using System.Linq;
using PromptGeneratorApp.Models;

namespace PromptGeneratorApp.Data;

public class PromptRepository
{
    private readonly PromptDatabase _database;

    public PromptRepository(PromptDatabase database)
    {
        _database = database;
    }

    public async Task<IReadOnlyList<Prompt>> GetPromptsAsync()
    {
        await _database.InitializeAsync();
        var records = await _database.GetPromptsAsync();
        return records.Select(Prompt.FromRecord).ToList();
    }

    public async Task AddPromptAsync(Prompt prompt)
    {
        await _database.InitializeAsync();
        var record = prompt.ToRecord();
        await _database.SavePromptAsync(record);
    }
}
