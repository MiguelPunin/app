using PromptGeneratorApp.Data;

namespace PromptGeneratorApp.Models;

public record Prompt(string Title, string Context, string Language)
{
    public static Prompt FromRecord(PromptRecord record) => new(record.Title, record.Context, record.Language);

    public PromptRecord ToRecord() => new()
    {
        Title = Title,
        Context = Context,
        Language = Language
    };
}
