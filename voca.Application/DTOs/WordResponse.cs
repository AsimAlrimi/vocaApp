namespace voca.Application.DTOs;

public class WordResponse
{
    public Guid Id { get; set; }

    public string Text { get; set; } = string.Empty;

    public string? Phonetic { get; set; }

    public string PartOfSpeech { get; set; } = string.Empty;

    public string Definition { get; set; } = string.Empty;

    public string? Example { get; set; }

    public string? Origin { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<string> Synonyms { get; set; } = new();
}
