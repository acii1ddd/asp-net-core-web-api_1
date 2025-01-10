using BLL.DTOs;
using FluentResults;

namespace BLL.ServicesInterfaces
{
    public interface IAuthorService
    {
        public Task<List<AuthorDTO>> GetAll();

        public Task<Result<AuthorDTO>> Add(AuthorDTO authorDTO);

        public Task<List<AuthorDTO>> GetAllWithBooks();

        public Task<AuthorDTO?> GetById(Guid id);

        public Task<List<AuthorDTO>> GetByFilter(string firstName, string lastName);

        public Task<Result<List<AuthorDTO>>> GetByPage(int page, int pageSize);

        public Task<Result<Guid>> UpdateFull(AuthorDTO author);

        public Task<Result<Guid>> UpdatePartial(AuthorDTO author);

        public Task<Result<AuthorDTO>> DeleteById(Guid id);
    }
}
