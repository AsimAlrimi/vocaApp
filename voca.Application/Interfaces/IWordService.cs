using voca.Application.DTOs;

namespace voca.Application.Interfaces;

public interface IWordService
{
    Task<WordResponse> CreateAsync(CreateWordRequest request);

    Task<WordResponse?> GetByIdAsync(Guid id);
                
    Task<List<WordResponse>> GetAllAsync();

    Task<bool> DeleteAsync(Guid id);
}