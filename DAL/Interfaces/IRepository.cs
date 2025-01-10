using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IRepository<T>
    {
        public Task<List<T>> GetAll();

        public Task<Author?> GetById(Guid id);

        public Task<List<Author>> GetByPage(int page, int pageSize);

        public Task<Author> Add(Author author);
        
        public Task<T?> DeleteById(Guid id);

        public Task<Guid> Update(Author author);
    }
}
