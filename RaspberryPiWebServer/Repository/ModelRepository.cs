using RaspberryPiWebServer.Data;
using RaspberryPiWebServer.Models;

namespace RaspberryPiWebServer.Repository
{
    public class ModelRepository : IEntityRepository<Model>
    {
        private readonly ApplicationDbContext _context;

        public ModelRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Model> Add(Model entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(Model entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Model> Get(object id)
        {
            return await _context.Models.FindAsync(id);
        }

        public IQueryable<Model> GetAll()
        {
            return _context.Models;
        }

        public async Task Update(Model entity)
        {
            _context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}