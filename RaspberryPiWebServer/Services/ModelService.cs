using RaspberryPiWebServer.Models;
using RaspberryPiWebServer.Repository;

namespace RaspberryPiWebServer.Services
{
    public class ModelService : IEntityService<Model>
    {
        private readonly IEntityRepository<Model> _repository;

        public ModelService(IEntityRepository<Model> repository)
        {
            _repository = repository;
        }
        public async Task<Model> Add(Model entity)
        {
            return await _repository.Add(entity);
        }

        public async Task Delete(Model entity)
        {
            await _repository.Delete(entity);
        }

        public async Task<Model> Get(object id)
        {
            return await _repository.Get(id);
        }

        public IQueryable<Model> GetAll()
        {
            return _repository.GetAll();
        }

        public async Task Update(Model entity)
        {
            await _repository.Update(entity);
        }
    }
}