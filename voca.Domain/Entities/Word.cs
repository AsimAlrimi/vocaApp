namespace voca.Domain.Entities;

public class Word
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Phonetic { get; set; }
    public string PartOfSpeech { get; set; } = string.Empty;
    public string Definition { get; set; } = string.Empty;
    public string? Example { get; set; }
    public string? Origin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<WordSynonym> Synonyms { get; set; } = new List<WordSynonym>();
    public ICollection<UserWord> UserWords { get; set; } = new List<UserWord>();
}