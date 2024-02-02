using System.Linq.Expressions;
using VeterinaryApplication.Models;

namespace VeterinaryApplication.Services
{
    public interface IPetService
    {
        IEnumerable<Pet> All { get; }

        Pet GetById(int id);

        void Add(Pet entity);
        void Update(Pet entity);
        void Delete(int entity);

        IEnumerable<Pet> GetAllWithInclude(params Expression<Func<Pet, object>>[] includeProperties);
    }
}
