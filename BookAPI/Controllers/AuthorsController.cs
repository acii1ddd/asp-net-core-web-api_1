using BLL.DTOs;
using BookAPI.ContractsDTOs.Requests;
using BookAPI.ContractsDTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using BLL.ServicesInterfaces;

namespace BookAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly ILogger<AuthorsController> _logger;
        private readonly IAuthorService _authorService;

        public AuthorsController(ILogger<AuthorsController> logger, IAuthorService authorService)
        {
            _logger = logger;
            this._authorService = authorService;
        }

        // /Authors (get all authors)
        [HttpGet]
        // ������ ���� ���� �� ������
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetAuthorResponse>))]
        public async Task<IActionResult> GetAll()
        {
            // authors only
            var authorsDTO = await _authorService.GetAll();

            var authors = authorsDTO.Select(author => new GetAuthorResponse(
                author.Id,
                author.FirstName,
                author.LastName,
                author.Email,
                author.BirthDate
            ));

            return Ok(authors);
        }

        // /Authors/WithBooks
        [HttpGet("WithBooks")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetAuthorWithBooksResponse>))]
        public async Task<IActionResult> GetAllWithBooks()
        {
            // authors with their books
            var authorsDTO = await _authorService.GetAllWithBooks();

            var authors = authorsDTO.Select(author => new GetAuthorWithBooksResponse(
                author.Id,
                author.FirstName,
                author.LastName,
                author.Email,
                author.BirthDate,
                author.Books.Select(book => new GetBookResponse(
                    book.Id,
                    book.Title,
                    book.ReleaseYear,
                    book.Price,
                    book.AuthorId
                )).ToList()
            ));

            return Ok(authors);
        }

        // /Authors/{Guid} (������ �� �����)
        [HttpGet("{authorId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAuthorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAuthorById([FromRoute] Guid authorId)
        {
            var authorDTO = await _authorService.GetById(authorId);

            if (authorDTO == null)
            {
                _logger.LogError($"Author with ID {authorId} not found.");
                return NotFound($"Author with ID {authorId} was not found.");
            }

            var author = new GetAuthorResponse(
                authorDTO.Id,
                authorDTO.FirstName,
                authorDTO.LastName,
                authorDTO.Email,
                authorDTO.BirthDate
            );

            return Ok(author);
        }

        // Authors/page?firstName=string&lastName=string
        [HttpGet("filter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetAuthorResponse>))]
        public async Task<IActionResult> GetAuthorByFilter([FromQuery] string? firstName, [FromQuery] string? lastName)
        {
            var authorsDTO = await _authorService.GetByFilter(firstName, lastName);

            if (authorsDTO.Count == 0)
            {
                _logger.LogWarning($"Author with firstName {firstName} and lastName {lastName} not found.");
                return Ok(new List<GetAuthorResponse>());
            }
            // mapping
            var results = authorsDTO.Select(author => new GetAuthorResponse(
                author.Id,
                author.FirstName,
                author.LastName,
                author.Email,
                author.BirthDate
            ));

            return Ok(results);
        }

        // Authors/page?page=int&pageSize=int
        [HttpGet("page")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetAuthorResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAuthorByPage([FromQuery] int page, [FromQuery] int pageSize)
        {
            var authorsDTOs = await _authorService.GetByPage(page, pageSize);

            if (authorsDTOs.IsFailed)
            {
                authorsDTOs.Errors.ForEach(error =>
                    _logger.LogError(error.Message));

                return BadRequest(authorsDTOs.Errors);
            }
                
            // mapping
            var results = authorsDTOs.Value.Select(author => new GetAuthorResponse(
                author.Id,
                author.FirstName,
                author.LastName,
                author.Email,
                author.BirthDate
            ));

            return Ok(results);
        }

        // Authors
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAuthorResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] AddAuthorRequest request)
        {
            // gives guid id in service
            var author = new AuthorDTO
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                BirthDate = request.BirthDate
            };
            
            // Result<T> type
            var createdAuthor = await _authorService.Add(author);
            if (createdAuthor.IsFailed)
            {
                createdAuthor.Errors.ForEach(error =>
                    _logger.LogError(error.Message));

                return BadRequest(createdAuthor.Errors);
            }

            var result = new GetAuthorResponse(
                createdAuthor.Value.Id,
                createdAuthor.Value.FirstName,
                createdAuthor.Value.LastName,
                createdAuthor.Value.Email,
                createdAuthor.Value.BirthDate
            );
            return Ok(result);
        }

        /// <summary>
        /// Full Author Update
        /// </summary>
        /// <param name="authorId">Author Id</param>
        /// <param name="request">Author full info</param>
        [HttpPut("{authorId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateAuthorResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> FullUpdate([FromRoute] Guid authorId, [FromBody] FullUpdateAuthorRequest request)
        {
            // date is not specified in json
            if (request.BirthDate == DateTime.MinValue)
            {
                return BadRequest("The BirthDate field is required.");
            }

            var author = new AuthorDTO
            {
                Id = authorId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                BirthDate = DateTime.SpecifyKind(request.BirthDate, DateTimeKind.Utc) // in utc format
            };

            var result = await _authorService.UpdateFull(author);
            if (result.IsFailed)
            {
                result.Errors.ForEach(error =>
                    _logger.LogError(error.Message));

                return BadRequest(result.Errors);
            }

            var response = new UpdateAuthorResponse(result.Value);
            return Ok(response);
        }

        /// <summary>
        /// Partial Author Update
        /// </summary>
        /// <param name="authorId">Author Id</param>
        /// <param name="request">Author info to update</param>
        [HttpPatch("{authorId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateAuthorResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PartialUpdate([FromRoute] Guid authorId, [FromBody] PartialUpdateAuthorRequest request)
        {
            // ��� null ���� �� ������� ��� ���� (������ �� ���������)
            var author = new AuthorDTO
            {
                Id = authorId,
                FirstName = request.FirstName ?? string.Empty,
                LastName = request.LastName ?? string.Empty,
                Email = request.Email ?? string.Empty,
                BirthDate = request.BirthDate ?? DateTime.MinValue
            };

            var result = await _authorService.UpdatePartial(author);
            if (result.IsFailed)
            {
                result.Errors.ForEach(error =>
                    _logger.LogError(error.Message));

                return BadRequest(result.Errors);
            }

            var response = new UpdateAuthorResponse(result.Value);
            return Ok(response);
            
        }

        // Authors/{Guid}
        [HttpDelete("{authorId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAuthorResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] Guid authorId)
        {
            var deletedAuthor = await _authorService.DeleteById(authorId);

            if (deletedAuthor.IsFailed)
            {
                deletedAuthor.Errors.ForEach(error =>
                                    _logger.LogError(error.Message));

                return BadRequest(deletedAuthor.Errors);
            }

            var result = new GetAuthorResponse(
                deletedAuthor.Value.Id,
                deletedAuthor.Value.FirstName,
                deletedAuthor.Value.LastName,
                deletedAuthor.Value.Email,
                deletedAuthor.Value.BirthDate
            );
            return Ok(result);
        }
    }
}
