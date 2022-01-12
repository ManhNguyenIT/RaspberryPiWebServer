using RaspberryPiWebServer.Models;
using RaspberryPiWebServer.Repository;

namespace RaspberryPiWebServer.Services
{
    public class HistoryService : IEntityService<History>
    {
        private readonly IEntityRepository<History> _repository;

        public HistoryService(IEntityRepository<History> repository)
        {
            _repository = repository;
        }
        public async Task<History> Add(History entity)
        {
            return await _repository.Add(entity);
        }

        public async Task Delete(History entity)
        {
            await _repository.Delete(entity);
        }

        public async Task<History> Get(long id)
        {
            return await _repository.Get(id);
        }

        public IQueryable<History> GetAll()
        {
            return _repository.GetAll();
        }

        public async Task Update(History entity)
        {
            await _repository.Update(entity);
        }
    }
}