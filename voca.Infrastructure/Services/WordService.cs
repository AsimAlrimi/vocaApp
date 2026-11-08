using Microsoft.EntityFrameworkCore;
using voca.Application.DTOs;
using voca.Application.Interfaces;
using voca.Domain.Entities;
using voca.Infrastructure.Data;

namespace voca.Infrastructure.Services;

public class WordService : IWordService
{
    private readonly AppDbContext _context;

    public WordService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<WordResponse> CreateAsync(CreateWordRequest request)
    {
        var word = new Word
        {
            Id = Guid.NewGuid(),
            Text = request.Text,
            Phonetic = request.Phonetic,
            PartOfSpeech = request.PartOfSpeech,
            Definition = request.Definition,
            Example = request.Example,
            Origin = request.Origin,
            Synonyms = request.Synonyms
                .Where(synonym => !string.IsNullOrWhiteSpace(synonym))
                .Select(synonym => new WordSynonym
                {
                    Id = Guid.NewGuid(),
                    Text = synonym.Trim()
                })
                .ToList()
        };

        _context.Words.Add(word);

        await _context.SaveChangesAsync();

        return new WordResponse
        {
            Id = word.Id,
            Text = word.Text,
            Phonetic = word.Phonetic,
            PartOfSpeech = word.PartOfSpeech,
            Definition = word.Definition,
            Example = word.Example,
            Origin = word.Origin,
            CreatedAt = word.CreatedAt,
            Synonyms = word.Synonyms
                .Select(synonym => synonym.Text)
                .ToList()
        };
    }

    public async Task<List<WordResponse>> GetAllAsync()
    {
        return await _context.Words
            .Include(word => word.Synonyms)
            .Select(word => new WordResponse
            {
                Id = word.Id,
                Text = word.Text,
                Phonetic = word.Phonetic,
                PartOfSpeech = word.PartOfSpeech,
                Definition = word.Definition,
                Example = word.Example,
                Origin = word.Origin,
                CreatedAt = word.CreatedAt,
                Synonyms = word.Synonyms
                    .Select(synonym => synonym.Text)
                    .ToList()
            })
            .ToListAsync();
    }

    public async Task<WordResponse?> GetByIdAsync(Guid id)
    {
        return await _context.Words
            .AsNoTracking()
            .Where(word => word.Id == id)
            .Select(word => new WordResponse
            {
                Id = word.Id,
                Text = word.Text,
                Phonetic = word.Phonetic,
                PartOfSpeech = word.PartOfSpeech,
                Definition = word.Definition,
                Example = word.Example,
                Origin = word.Origin,
                CreatedAt = word.CreatedAt,

                Synonyms = word.Synonyms
                    .Select(synonym => synonym.Text)
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var word = await _context.Words.FindAsync(id);

        if (word is null)
        {
            return false;
        }

        _context.Words.Remove(word);
        await _context.SaveChangesAsync();

        return true;
    }

}
