using WebApp.Data;
using WebApp.Models;

namespace WebApp.Repository
{
    public class HistoryRepository : IEntityRepository<History>
    {
        private readonly ApplicationDbContext _context;

        public HistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<History> Add(History entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(History entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<History> Get(object id)
        {
            return await _context.Histories.FindAsync(id);
        }

        public IQueryable<History> GetAll()
        {
            return _context.Histories;
        }

        public async Task Update(History entity)
        {
            _context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}