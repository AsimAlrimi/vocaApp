using Microsoft.AspNetCore.Mvc;
using voca.Application.DTOs;
using voca.Application.Interfaces;

namespace voca.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WordsController : ControllerBase
{
    private readonly IWordService _wordService;

    public WordsController(IWordService wordService)
    {
        _wordService = wordService;
    }

    [HttpPost]
    public async Task<ActionResult<WordResponse>> Create(
        CreateWordRequest request)
    {
        var word = await _wordService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetAll),
            new { id = word.Id },
            word);
    }

    [HttpGet]
    public async Task<ActionResult<List<WordResponse>>> GetAll()
    {
        var words = await _wordService.GetAllAsync();

        return Ok(words);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WordResponse>> GetById(Guid id)
    {
        var word = await _wordService.GetByIdAsync(id);

        if(word is null)
        {
            return NotFound(new
            {
                message = "Word not found."
            });
        }

        return Ok(word);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var flag = await _wordService.DeleteAsync(id);
        
        if (flag)
        {
          return NoContent(); 
        }

        return NotFound(new
        {
            message = "Word not found."
        });

    }
}