using AutoMapper;
using BLL.DTOs;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class AuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ILogger<AuthorService> _logger;
        private readonly IMapper _mapper;

        public AuthorService(ILogger<AuthorService> logger, IAuthorRepository authorRepository, IMapper mapper)
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

        public async Task Add(AuthorDTO authorDTO)
        {
            var author = _mapper.Map<Author>(authorDTO);
            await _authorRepository.Add(author);
        }
    }
}
