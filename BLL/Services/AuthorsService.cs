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

        /// <summary>
        /// update method 
        /// </summary>
        
        public async Task<AuthorDTO> DeleteById(Guid id)
        {
            var author = await _authorRepository.DeleteById(id);
            if (author == null)
            {
                throw new InvalidOperationException($"Author with ID {id} not found.");
            }

            return _mapper.Map<AuthorDTO>(author);
        }
    }
}
