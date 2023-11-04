using Microsoft.EntityFrameworkCore;

namespace AcePacific.Data.RepositoryPattern.Implementations
{
    public class EntityFrameworkDataContext<TContext> : DbContext, IDataContextAsync where TContext : DbContext
    {
        #region Private Fields
        private readonly Guid _instanceId;
        bool _disposed;
        #endregion Private Fields

        public EntityFrameworkDataContext(DbContextOptions<TContext> options)
            : base(options)
        {
            _instanceId = Guid.NewGuid();

        }


        public async Task<int> SaveChangesAsync()
        {
            return await this.SaveChangesAsync(CancellationToken.None);
        }
    }
}
