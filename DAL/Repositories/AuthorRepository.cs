using DAL.Context;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly BookDbContext _dbContext;

        public AuthorRepository(BookDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Author>> GetAll()
        {
            return await _dbContext.Authors
                .AsNoTracking() // не отслеживаем изменения, так как мы ничего не изменяем, а просто получаем данные
                .ToListAsync();
        }

        public async Task<List<Author>> GetAllWithBooks()
        {
            return await _dbContext.Authors
                .Include(author => author.Books)
                .AsNoTracking()
                .ToListAsync();
        }

        // null либо Author, полученный по id
        public async Task<Author?> GetById(Guid id)
        {
            return await _dbContext.Authors
                .FirstOrDefaultAsync(author => author.Id == id);
        }

        // получение авторов по вхождению firstName и lastName(пустой список либо список Author's)
        public async Task<List<Author>> GetByFilter(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            {
                return new List<Author>();
            }

            var query = _dbContext.Authors.AsNoTracking();

            if (!string.IsNullOrEmpty(firstName))
            {
                query.Where(author => author.FirstName.Contains(firstName));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                query.Where(author => author.LastName.Contains(lastName));
            }

            return await query.ToListAsync();
        }

        // пагинация (получение по страницам) page - номер страницы, на которой нужно получить данные
        // pageSize - количество элементов на одной странице
        public async Task<List<Author>> GetByPage(int page, int pageSize)
        {
            return await _dbContext.Authors
                .AsNoTracking()
                .Skip((page - 1) * pageSize) // например для page = 5, то есть для получение данный с пятой страницы
                                             // нужно пропустить 4 страницы и все их элементы
                .Take(pageSize) // взять количество элементов, которые могут
                                // содержаться на одной странице - наша пятая страница
                .ToListAsync();
        }

        public async Task Add(Author author)
        {
            await _dbContext.Authors.AddAsync(author);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(Author author)
        {
            await _dbContext.Authors
                .Where(a => a.Id == author.Id) // каких нужно обновить
                .ExecuteUpdateAsync(s => s
                    .SetProperty(a => a.FirstName, author.FirstName)
                    .SetProperty(a => a.LastName, author.LastName)
                    .SetProperty(a => a.Email, author.Email)
                    .SetProperty(a => a.BirthDate, author.BirthDate)
                );
        }

        public async Task DeleteById(Guid id)
        {
            await _dbContext.Authors
                .Where(author => author.Id == id)
                .ExecuteDeleteAsync();
        }
    }
}
