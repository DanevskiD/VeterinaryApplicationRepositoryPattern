using System.Collections.Generic;
using System.Linq.Expressions;
using VeterinaryApplication.Models;

namespace VeterinaryApplication.Repository;

public interface IRepository<T>
{
    IEnumerable<T> All { get; }

    T GetById(int id);

    void Add(T entity);
    void Update(T entity);
    void Delete(int entity);

    IEnumerable<T> GetAllWithInclude(params Expression<Func<T, object>>[] includeProperties);
}
