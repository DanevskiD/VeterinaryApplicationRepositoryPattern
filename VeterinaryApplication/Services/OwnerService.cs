using System.Linq.Expressions;
using VeterinaryApplication.Models;
using VeterinaryApplication.Repository;

namespace VeterinaryApplication.Services
{
    public class OwnerService : IOwnerService
    {
        private readonly IRepository<Owner> _ownerRepository;

        public OwnerService(IRepository<Owner> ownerRepository)
        {
            _ownerRepository = ownerRepository;
        }

        public IEnumerable<Owner> All => _ownerRepository.All;

        public void Add(Owner entity)
        {
            _ownerRepository.Add(entity);
        }

        public void Delete(int entity)
        {
            _ownerRepository.Delete(entity);
        }

        public IEnumerable<Owner> GetAllWithInclude(params Expression<Func<Owner, object>>[] includeProperties)
        {
            return _ownerRepository.GetAllWithInclude(includeProperties);
        }

        public Owner GetById(int id)
        {
            return _ownerRepository.GetById(id);
        }

        public void Update(Owner entity)
        {
            _ownerRepository.Update(entity);
        }
    }
}
