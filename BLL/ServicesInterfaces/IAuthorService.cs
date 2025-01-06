using BLL.DTOs;

namespace BLL.ServicesInterfaces
{
    public interface IAuthorService
    {
        public Task<List<AuthorDTO>> GetAll();

        public Task<AuthorDTO> Add(AuthorDTO authorDTO);

        public Task<List<AuthorDTO>> GetAllWithBooks();

        public Task<AuthorDTO?> GetById(Guid id);

        public Task<List<AuthorDTO>> GetByFilter(string firstName, string lastName);

        public Task<List<AuthorDTO>> GetByPage(int page, int pageSize);

        public Task<AuthorDTO> DeleteById(Guid id);
    }
}
