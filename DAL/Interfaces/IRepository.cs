using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IRepository<T>
    {
        public Task<List<T>> GetAll();

        public Task<T?> GetById(Guid id);

        public Task<List<T>> GetByPage(int page, int pageSize);

        public Task<T> Add(T author);
        
        public Task<T?> DeleteById(Guid id);

        public Task<Guid> Update(T author);
    }
}
