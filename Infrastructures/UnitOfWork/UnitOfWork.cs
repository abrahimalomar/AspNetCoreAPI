using Core.interfaces;
using Infrastructures.Repository;


namespace Infrastructures.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            products = new GenericRepository<Product>(_context);
            categories = new GenericRepository<Category>(_context);

        }
        public IGenericRepository<Product> products { get; private set; }
        public IGenericRepository<Category> categories { get; private set; }



        public void Dispose()
        {
            _context.Dispose();
        }

        public void save()
        {
            _context.SaveChanges();
        }
    }
}
