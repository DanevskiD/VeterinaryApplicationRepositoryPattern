using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VeterinaryApplication.Data;

namespace VeterinaryApplication.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _entities;

    public Repository(ApplicationDbContext context)
    {
        this._context = context;
        _entities = context.Set<T>();
    }

    public IEnumerable<T> All => _entities.ToList();

    public void Add(T entity)
    {
        _entities.Add(entity);
        _context.SaveChanges();
    }

    public void Delete(int entity)
    {
        var entityToDelete = _entities.Find(entity);

        if (entityToDelete != null)
        {
            _entities.Remove(entityToDelete);
            _context.SaveChanges();
        }
    }

    public IEnumerable<T> GetAllWithInclude(params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query=_entities.AsQueryable();

        foreach (var includeProperty in includeProperties)
        {
            query=query.Include(includeProperty);
        }

        return query.ToList();
    }

    public T GetById(int id)
    {
        return _entities.Find(id);
    }

    public void Update(T entity)
    {
        _entities.Update(entity);
        _context.SaveChanges();
    }
}
