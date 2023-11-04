namespace AcePacific.Data.RepositoryPattern
{
    public interface IRepositoryAsync<T> : IRepository<T> where T : class
    {
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<T> FindAsync(params object[] keyValues);
        Task<T> FindAsync(CancellationToken cancellationToken, params object[] keyValues);
        Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues);
        Task InsertAsync(T entity, bool saveNow = true);
        Task InsertRangeAsync(IEnumerable<T> entities, bool saveNow = true);
        Task UpdateAsync(T entity, bool saveNow = true);
        Task UpdateRangeAsync(IEnumerable<T> entities, bool updateNow = true);
        Task<bool> DeleteAsync(params object[] keyValues);
    }
}
