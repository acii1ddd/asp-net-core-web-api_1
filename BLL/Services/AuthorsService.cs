using AutoMapper;
using BLL.DTOs;
using BLL.ServicesInterfaces;
using DAL.Entities;
using DAL.Interfaces;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class AuthorsService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILogger<AuthorsService> _logger;
        private readonly IMapper _mapper;

        public AuthorsService(ILogger<AuthorsService> logger, IAuthorRepository authorRepository, IMapper mapper)
        {
            this._authorRepository = authorRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<AuthorDTO>> GetAll()
        {
            var authors = await _authorRepository.GetAll();
            return _mapper.Map<List<AuthorDTO>>(authors);
        }

        public async Task<List<AuthorDTO>> GetAllWithBooks()
        {
            var authors = await _authorRepository.GetAllWithBooks();
            return _mapper.Map<List<AuthorDTO>>(authors);
        }

        // null or author
        public async Task<AuthorDTO?> GetById(Guid id)
        {
            var author = await _authorRepository.GetById(id);
            return author == null ? null : _mapper.Map<AuthorDTO>(author);
        }

        // get by filter
        public async Task<List<AuthorDTO>> GetByFilter(string firstName, string lastName)
        {
            // all filters is invalid
            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            {
                return new List<AuthorDTO>(); 
            }

            var authors = await _authorRepository.GetByFilter(firstName, lastName);
            return _mapper.Map<List<AuthorDTO>>(authors);
        }

        // pagination
        public async Task<Result<List<AuthorDTO>>> GetByPage(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return Result.Fail("Page and pageSize must be greater than zero.");
            }

            var authors = await _authorRepository.GetByPage(page, pageSize);
            return _mapper.Map<List<AuthorDTO>>(authors);
        }

        // with result pattern
        public async Task<Result<AuthorDTO>> Add(AuthorDTO authorDTO)
        {
            var existingAuthor = await _authorRepository.FindByEmail(authorDTO.Email);
            if (existingAuthor != null)
            {
                return Result.Fail(new Error($"Author with email '{authorDTO.Email}' already exists."));
            }

            authorDTO.Id = Guid.NewGuid();
            var author = await _authorRepository.Add(_mapper.Map<Author>(authorDTO));

            // AuthorDTO сам неявно оборачивается в Result при возврате (implicit operator)
            return _mapper.Map<AuthorDTO>(author);
        }

        public async Task<Result<Guid>> UpdateFull(AuthorDTO author)
        {
            var existingAuthorById = await _authorRepository.GetById(author.Id);
            if (existingAuthorById == null)
            {
                return Result.Fail(new Error($"Author with ID '{author.Id}' not found."));
            }

            // если обновляем на тот же email - то пропускаем
            if (!string.Equals(existingAuthorById.Email, author.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existingAuthorByEmail = await _authorRepository.FindByEmail(author.Email);

                if (existingAuthorByEmail != null)
                {
                    return Result.Fail(new Error($"Author with email '{author.Email}' already exists."));
                }
            }

            // is valid
            var id = await _authorRepository.Update(_mapper.Map<Author>(author));
            return Result.Ok(id); // явно оборачиваем (более читаемо)
        }

        public async Task<Result<Guid>> UpdatePartial(AuthorDTO author)
        {
            var existingAuthor = await _authorRepository.GetById(author.Id);
            if (existingAuthor == null)
            {
                return Result.Fail(new Error($"Author with ID '{author.Id}' not found."));
            }

            // update existing fields
            if (!string.IsNullOrEmpty(author.FirstName))
            {
                existingAuthor.FirstName = author.FirstName;
            }
            if (!string.IsNullOrEmpty(author.LastName))
            {
                existingAuthor.LastName = author.LastName;
            }
            if (!string.IsNullOrEmpty(author.Email))
            {
                // если обновляем на тот же email - то пропускаем
                if (!string.Equals(existingAuthor.Email, author.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var existingAuthorByEmail = await _authorRepository.FindByEmail(author.Email);

                    if (existingAuthorByEmail != null)
                    {
                        return Result.Fail(new Error($"Author with email '{author.Email}' already exists."));
                    }
                }

                existingAuthor.Email = author.Email;
            }
            if (author.BirthDate != DateTime.MinValue)
            {
                // для DateTime в timestamp with timezone
                existingAuthor.BirthDate = DateTime.SpecifyKind(author.BirthDate, DateTimeKind.Utc);
            }

            // is valid
            var id = await _authorRepository.Update(existingAuthor);
            return Result.Ok(id);
        }

        public async Task<Result<AuthorDTO>> DeleteById(Guid authorId)
        {
            var author = await _authorRepository.DeleteById(authorId);
            if (author == null)
            {
                return Result.Fail(new Error($"Author with ID '{authorId}' not found."));
            }

            return _mapper.Map<AuthorDTO>(author);
        }
    }
}
