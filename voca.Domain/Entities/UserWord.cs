namespace voca.Domain.Entities;

public class UserWord
{
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public Guid WordId { get; set; }

    public Word Word { get; set; } = null!;

    public DateTime SavedAt { get; set; } = DateTime.UtcNow;
}