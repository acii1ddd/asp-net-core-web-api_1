using BLL.DTOs;
using Microsoft.AspNetCore.Mvc;
using BLL.ServicesInterfaces;
using BookAPI.Cache;
using BookAPI.Contracts.Responses;
using BookAPI.Contracts.Requests;
using AutoMapper;

namespace BookAPI.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    [Route("authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly ILogger<AuthorsController> _logger;
        private readonly IAuthorService _authorService;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public AuthorsController(ILogger<AuthorsController> logger, IAuthorService authorService, ICacheService cacheService, IMapper mapper)
        {
            _logger = logger;
            _authorService = authorService;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        // /Authors (all authors)
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetAuthorResponse>))]
        public async Task<IActionResult> GetAll()
        {
            // cache
            var cachAuthors = await _cacheService.GetDataAsync<IEnumerable<AuthorDTO>>("Authors");
            if (cachAuthors != null)
            {
                return Ok(_mapper.Map<IEnumerable<GetAuthorResponse>>(cachAuthors));
            }

            // db
            var dbAuthors = await _authorService.GetAll();
            
            // кэшируем результат
            await _cacheService.SetDataAsync("Authors", dbAuthors, DateTimeOffset.Now.AddMinutes(5));

            return Ok(_mapper.Map<IEnumerable<GetAuthorResponse>>(dbAuthors));
        }

        // /Authors/WithBooks
        [HttpGet("WithBooks")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetAuthorWithBooksResponse>))]
        public async Task<IActionResult> GetAllWithBooks()
        {
            var cachAuthors = await _cacheService.GetDataAsync<IEnumerable<AuthorDTO>>("Authors");
            if (cachAuthors != null)
            {
                return Ok(_mapper.Map<IEnumerable<GetAuthorWithBooksResponse>>(cachAuthors));
            }

            // authors with their books
            var authors = await _authorService.GetAllWithBooks(); 
            
            await _cacheService.SetDataAsync("Authors", authors, DateTimeOffset.Now.AddMinutes(5));
            
            return Ok(_mapper.Map<IEnumerable<GetAuthorWithBooksResponse>>(authors));
        }

        // /Authors/{Guid}
        [HttpGet("{authorId:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAuthorResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAuthorById([FromRoute] Guid authorId)
        {
            var cacheAuthor = await _cacheService.GetDataAsync<AuthorDTO>(authorId.ToString());
            if (cacheAuthor != null)
            {
                return Ok(_mapper.Map<GetAuthorResponse>(cacheAuthor));
            }

            var author = await _authorService.GetById(authorId);

            if (author == null)
            {
                _logger.LogWarning($"Author with ID {authorId} not found.");
                return NotFound($"Author with ID {authorId} was not found.");
            }

            await _cacheService.SetDataAsync(authorId.ToString(), author, DateTimeOffset.Now.AddMinutes(5));
            
            return Ok(_mapper.Map<GetAuthorResponse>(author));
        }

        // Authors/page?firstName=string&lastName=string
        [HttpGet("filter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetAuthorResponse>))]
        public async Task<IActionResult> GetAuthorByFilter([FromQuery] string? firstName, [FromQuery] string? lastName)
        {
            // no cache here
            var authors = await _authorService.GetByFilter(firstName, lastName);

            if (authors.Count == 0)
            {
                _logger.LogWarning($"Author with firstName {firstName ?? "\'null\'"} and lastName {lastName ?? "\'null\'"} not found.");
                return Ok(new List<GetAuthorResponse>());
            }
            // mapping
            return Ok(_mapper.Map<IEnumerable<GetAuthorResponse>>(authors));
        }

        // Authors/page?page=int&pageSize=int
        [HttpGet("page")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetAuthorResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAuthorByPage([FromQuery] int page, [FromQuery] int pageSize)
        {
            // no cache here
            var authors = await _authorService.GetByPage(page, pageSize);

            if (authors.IsFailed)
            {
                authors.Errors.ForEach(error =>
                    _logger.LogError(error.Message));

                return BadRequest(authors.Errors);
            }

            // mapping
            return Ok(_mapper.Map<IEnumerable<GetAuthorResponse>>(authors.Value));
        }

        // Authors
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAuthorResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] AddAuthorRequest request)
        {
            // date is not specified in json
            if (request.BirthDate == DateTime.MinValue)
            {
                return BadRequest("The BirthDate field is required.");
            }

            // gives guid id in service
            var author = _mapper.Map<AuthorDTO>(request);
            author.BirthDate = DateTime.SpecifyKind(request.BirthDate, DateTimeKind.Utc); // with utc kind

            // Result<T> type
            var createdAuthor = await _authorService.Add(author);
            if (createdAuthor.IsFailed)
            {
                createdAuthor.Errors.ForEach(error =>
                    _logger.LogError(error.Message));

                return BadRequest(createdAuthor.Errors);
            }

            // update cache
            await _cacheService.RemoveDataAsync("Authors");
            
            return Ok(_mapper.Map<GetAuthorResponse>(createdAuthor.Value));
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

            var author = _mapper.Map<AuthorDTO>(request);
            author.Id = authorId;

            // чтобы entity мог впихнуть DateTime в тип timestamp with timezone в postgres
            author.BirthDate = DateTime.SpecifyKind(request.BirthDate, DateTimeKind.Utc); // with utc kind

            var result = await _authorService.UpdateFull(author);
            if (result.IsFailed)
            {
                result.Errors.ForEach(error =>
                    _logger.LogError(error.Message));

                return BadRequest(result.Errors);
            }

            await _cacheService.RemoveListAsync(new List<string>
            {
                "Authors",
                result.Value.ToString()
            });

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
            var author = _mapper.Map<AuthorDTO>(request);
            author.Id = authorId;

            var result = await _authorService.UpdatePartial(author);
            if (result.IsFailed)
            {
                result.Errors.ForEach(error =>
                    _logger.LogError(error.Message));

                return BadRequest(result.Errors);
            }

            await _cacheService.RemoveListAsync(new List<string>
            {
                "Authors",
                result.Value.ToString()
            });

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
            
            //update cache
            await _cacheService.RemoveListAsync(new List<string>
            { 
                "Authors",
                authorId.ToString() 
            });

            return Ok(_mapper.Map<GetAuthorResponse>(deletedAuthor.Value));
        }
    }
}
