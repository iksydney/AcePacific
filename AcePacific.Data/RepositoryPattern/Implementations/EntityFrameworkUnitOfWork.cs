using AcePacific.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AcePacific.Data.RepositoryPattern.Implementations
{
    public class EntityFrameworkUnitOfWork : IUnitOfWorkAsync
    {
        #region Private Fields

        private DbContext _context;
        protected readonly IServiceProvider ServiceProvider;

        #endregion Private Fields

        #region Constructor/Dispose

        public EntityFrameworkUnitOfWork(IDataContextAsync dataContext, IServiceProvider serviceProvider)
        {
            _context = (DbContext)dataContext;
            ServiceProvider = serviceProvider;
        }


        public int SaveChanges()
        {
            CheckDisposed();
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            CheckDisposed();
            var r = _context.SaveChanges();
            return await Task.FromResult(r);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : class
        {
            CheckDisposed();
            var repositoryType = typeof(IRepositoryAsync<TEntity>);
            var repository = (IRepositoryAsync<TEntity>)ServiceProvider.GetService(repositoryType);
            if (repository == null)
            {
                throw new RepositoryNotFoundExceptions(repositoryType.Name,
                    $"Repository {repositoryType.Name} not found in the IOC container. Check if it is registered during startup.");
            }

            ((IRepositoryInjection)repository)?.SetContext(_context);
            return repository;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            CheckDisposed();
            var repositoryType = typeof(IRepository<TEntity>);
            var repository = (IRepository<TEntity>)ServiceProvider.GetService(repositoryType);
            if (repository == null)
            {
                throw new RepositoryNotFoundExceptions(repositoryType.Name,
                    $"Repository {repositoryType.Name} not found in the IOC container. Check if it is registered during startup.");
            }

            // ((IRepositoryInjection) repository).SetContext(_context);
            return repository;
        }

        protected bool _isDisposed;

        protected void CheckDisposed()
        {
            if (_isDisposed) throw new ObjectDisposedException("The UnitOfWork is already disposed and cannot be used anymore.");
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    if (_context != null)
                    {
                        if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                        {
                            _context.Database.CurrentTransaction?.Dispose();
                            _context.Database.GetDbConnection().Close();
                        }
                        else
                        {
                            _context.Database.CurrentTransaction?.Dispose();
                            _context.Dispose();
                            _context = null;
                        }
                    }
                }
            }
            _isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~EntityFrameworkUnitOfWork()
        {
            Dispose(false);
        }


        #endregion Constuctor/Dispose


         
        #region Unit of Work Transactions

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            if (_context.Database.GetDbConnection().State != ConnectionState.Open)
            {
                _context.Database.GetDbConnection().Open();
            }
            _context.Database.BeginTransaction();
        }

        public void Commit()
        {
            _context.Database.CommitTransaction();
        }

        public void Rollback()
        {
            _context.Database.RollbackTransaction();
        }

        public void DisposeTransaction()
        {

        }


        #endregion
    }
}