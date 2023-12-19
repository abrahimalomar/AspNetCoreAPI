using Core.interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Infrastructures.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected AppDbContext _context;
        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }
        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public T GetById(int Id)
        {
            return _context.Set<T>().Find(Id);
        }
        public void Create(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }

            _context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }

            _context.Set<T>().Remove(entity);
        }

        public void DeleteRange(IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                var model = _context.Set<T>().Find(id);
                if (model != null)
                {
                    _context.Set<T>().Remove(model);
                }
               
            }
        }

        public void Update(T entity)
        { 
          _context.Set<T>().Update(entity);   
        }


        public IEnumerable<T> GetAllWithIncludes(params Expression<Func<T, object>>[] includes)
        {

            IQueryable<T> query = _context.Set<T>();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query.ToList();

        }
    }
}
