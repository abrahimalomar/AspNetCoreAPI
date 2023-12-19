
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.interfaces
{
   public interface IUnitOfWork:IDisposable
    {
        IGenericRepository<Product> products { get; }
        IGenericRepository<Category> categories { get; }
        void save();
    }
}
