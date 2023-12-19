using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Core.interfaces
{
  public  interface IGenericRepository<T> where T:class
    {
        T GetById(int Id);
        void Create(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<int> ids);
        void Update(T entity);
        IEnumerable<T> GetAll();
        //T Find(Expression<Func<T, bool>> criteria, string[] includes);
        IEnumerable<T> GetAllWithIncludes(params Expression<Func<T, object>>[] includes);

    }
}
