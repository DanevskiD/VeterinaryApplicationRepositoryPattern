using System.Linq.Expressions;
using VeterinaryApplication.Models;
using VeterinaryApplication.Repository;

namespace VeterinaryApplication.Services
{
    public class PetService : IPetService
    {
        private readonly IRepository<Pet> _petRepository;

        public PetService(IRepository<Pet> petRepository)
        {
            _petRepository = petRepository;
        }

        public IEnumerable<Pet> All => _petRepository.All;

        public void Add(Pet entity)
        {
            _petRepository.Add(entity);
        }

        public void Delete(int entity)
        {
            _petRepository.Delete(entity);
        }

        public IEnumerable<Pet> GetAllWithInclude(params Expression<Func<Pet, object>>[] includeProperties)
        {
            return _petRepository.GetAllWithInclude(includeProperties);
        }

        public Pet GetById(int id)
        {
            return _petRepository.GetById(id);
        }

        public void Update(Pet entity)
        {
            _petRepository.Update(entity);
        }
    }
}
