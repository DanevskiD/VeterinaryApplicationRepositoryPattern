using System.Linq.Expressions;
using VeterinaryApplication.Models;

namespace VeterinaryApplication.Services
{
    public interface IOwnerService
    {
        IEnumerable<Owner> All { get; }

        Owner GetById(int id);

        void Add(Owner entity);
        void Update(Owner entity);
        void Delete(int entity);

        IEnumerable<Owner> GetAllWithInclude(params Expression<Func<Owner, object>>[] includeProperties);
    }
}
