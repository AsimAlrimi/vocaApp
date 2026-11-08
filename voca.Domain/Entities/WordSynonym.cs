namespace voca.Domain.Entities;

public class WordSynonym
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;

    public Guid WordId { get; set; }
    public Word Word { get; set; } = null!;
}