using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IAuthorRepository : IRepository<Author>
    {
        public Task<List<Author>> GetAllWithBooks();

        public Task<List<Author>> GetByFilter(string firstName, string lastName);
    }
}
