using System.Data;

namespace AcePacific.Data.RepositoryPattern
{
    public interface IUnitOfWork
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Unspecified);
        void Commit();
        void Rollback();
        IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : class;

    }
}
