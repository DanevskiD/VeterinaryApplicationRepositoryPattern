using System.Linq.Expressions;
using VeterinaryApplication.Models;
using VeterinaryApplication.Repository;

namespace VeterinaryApplication.Services
{
    public class VaccineService : IVaccineService
    {
        private readonly IRepository<Vaccine> _vaccineRepository;

        public VaccineService(IRepository<Vaccine> vaccineRepository)
        {
            _vaccineRepository = vaccineRepository;
        }

        public IEnumerable<Vaccine> All => _vaccineRepository.All;

        public void Add(Vaccine entity)
        {
            _vaccineRepository.Add(entity);
        }

        public void Delete(int entity)
        {
            _vaccineRepository.Delete(entity);
        }

        public IEnumerable<Vaccine> GetAllWithInclude(params Expression<Func<Vaccine, object>>[] includeProperties)
        {
            return _vaccineRepository.GetAllWithInclude(includeProperties);
        }

        public Vaccine GetById(int id)
        {
            return _vaccineRepository.GetById(id);
        }

        public void Update(Vaccine entity)
        {
            _vaccineRepository.Update(entity);
        }
    }
}
