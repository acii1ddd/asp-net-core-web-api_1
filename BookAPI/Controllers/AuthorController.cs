using BLL.DTOs;
using BLL.Services;
using BookAPI.ContractsDTOs;
using Microsoft.AspNetCore.Mvc;

namespace BookAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly ILogger<AuthorController> _logger;
        private readonly AuthorService _authorService;

        public AuthorController(ILogger<AuthorController> logger, AuthorService authorService)
        {
            _logger = logger;
            this._authorService = authorService;
        }

        [HttpGet("GetAll")]
        public async Task<IEnumerable<AuthorRequest>> GetAll()
        {
            var authorsDTO = await _authorService.GetAll();

            var authors = authorsDTO.Select(author => new AuthorRequest(
                author.FirstName,
                author.LastName,
                author.Email,
                author.BirthDate
            ));

            return authors;
        }

        // Author/Add будет url
        [HttpPost("Add")]
        public async Task<IActionResult> Add(AuthorRequest request)
        {
            var author = new AuthorDTO
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                BirthDate = request.BirthDate
            };

            await _authorService.Add(author);
            return Ok();
        }
    }
}
