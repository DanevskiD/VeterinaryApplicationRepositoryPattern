using System.Linq.Expressions;
using VeterinaryApplication.Models;

namespace VeterinaryApplication.Services
{
    public interface IVaccineService
    {
        IEnumerable<Vaccine> All { get; }

        Vaccine GetById(int id);

        void Add(Vaccine entity);
        void Update(Vaccine entity);
        void Delete(int entity);

        IEnumerable<Vaccine> GetAllWithInclude(params Expression<Func<Vaccine, object>>[] includeProperties);
    }
}
