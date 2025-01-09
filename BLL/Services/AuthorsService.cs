using AutoMapper;
using BLL.DTOs;
using BLL.ServicesInterfaces;
using DAL.Entities;
using DAL.Interfaces;
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
        public async Task<List<AuthorDTO>> GetByPage(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                throw new ArgumentException("Page and pageSize must be greater than zero.");
            }

            var authors = await _authorRepository.GetByPage(page, pageSize);
            return _mapper.Map<List<AuthorDTO>>(authors);
        }

        public async Task<AuthorDTO> Add(AuthorDTO authorDTO)
        {
            var existingAuthor = await _authorRepository.FindByEmail(authorDTO.Email);
            // лучше переделать это на паттерн result
            if (existingAuthor != null)
            {
                throw new InvalidOperationException($"Author with email {authorDTO.Email} already exists.");
            }

            authorDTO.Id = Guid.NewGuid();
            var author = await _authorRepository.Add(_mapper.Map<Author>(authorDTO));
            return _mapper.Map<AuthorDTO>(author);
        }

        public async Task UpdateFull(AuthorDTO author)
        {
            var existingAuthorById = await _authorRepository.GetById(author.Id);
            // лучше переделать это на паттерн result
            if (existingAuthorById == null)
            {
                throw new InvalidOperationException($"Author with ID {author.Id} not found.");
            }

            // если обновляем на тот же email - то пропускаем
            if (!string.Equals(existingAuthorById.Email, author.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existingAuthorByEmail = await _authorRepository.FindByEmail(author.Email);

                if (existingAuthorByEmail != null)
                {
                    throw new InvalidOperationException($"Author with email {author.Email} already exists.");
                }
            }

            // is valid
            await _authorRepository.Update(_mapper.Map<Author>(author));
        }

        public async Task UpdatePartial(AuthorDTO author)
        {
            var existingAuthor = await _authorRepository.GetById(author.Id);
            // лучше переделать это на паттерн result
            if (existingAuthor == null)
            {
                throw new InvalidOperationException($"Author with ID {author.Id} not found.");
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
                        throw new InvalidOperationException($"Author with email {author.Email} already exists.");
                    }
                }

                existingAuthor.Email = author.Email;
            }
            if (author.BirthDate != DateTime.MinValue)
            {
                existingAuthor.BirthDate = DateTime.SpecifyKind(author.BirthDate, DateTimeKind.Utc);
            }

            // is valid
            await _authorRepository.Update(existingAuthor);
        }

        public async Task<AuthorDTO> DeleteById(Guid authorId)
        {
            var author = await _authorRepository.DeleteById(authorId);
            if (author == null)
            {
                throw new InvalidOperationException($"Author with ID {authorId} not found.");
            }

            return _mapper.Map<AuthorDTO>(author);
        }
    }
}
