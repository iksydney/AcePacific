using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AcePacific.Data.RepositoryPattern.Implementations
{
    public class Repository<T> : IRepositoryAsync<T> where T : class
    {
        protected readonly IDataContextAsync Context;
        private readonly IUnitOfWork _unitOfWork;
        protected readonly DbSet<T> DbSet;

        public Repository(IDataContextAsync context, IUnitOfWork unitOfWork)
        {
            Context = context;
            _unitOfWork = unitOfWork;

            if (context is DbContext dbContext)
            {
                DbSet = dbContext.Set<T>();
            }
        }

        public IQueryable<T> Table => Queryable();
        public T Find(long id)
        {
            return DbSet.Find(id);
        }

        public T Find(string id)
        {
            return DbSet.Find(id);
        }

        public T Find(int id)
        {
            return DbSet.Find(id);
        }

        public T Find(Guid id)
        {
            return DbSet.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return Table;
        }

        public T Find(Expression<Func<T, bool>> predicate)
        {
            return Table.SingleOrDefault(predicate);
        }
        public virtual Task<T> FindAsync(params object[] keyValues)
        {
            return FindAsync(CancellationToken.None, keyValues);
        }

        public async Task<T> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return await DbSet.FindAsync(cancellationToken, keyValues);
        }
        public void insert(T entity, bool saveNow = true)
        {
            ((DbContext)Context).Entry(entity).State = EntityState.Added;
            if (saveNow)
                Context.SaveChanges();
        }
        public async Task InsertAsync(T entity, bool saveNow = true)
        {
            ((DbContext)Context).Entry(entity).State = EntityState.Added;
            if (saveNow)
                await Context.SaveChangesAsync();
        }
        public void insertRange(IEnumerable<T> entities, bool saveNow = true)
        {
            foreach (var entity in entities)
                insert(entity);
            if (saveNow)
                Context.SaveChanges();
        }
        public async Task InsertRangeAsync(IEnumerable<T> entities, bool saveNow = true)
        {
            foreach (var entity in entities)
                insert(entity);
            if (saveNow)
                await Context.SaveChangesAsync();
        }
        public virtual void SaveChanges()
        {
            Context.SaveChanges();
        }
        public virtual Task<int> SaveChangesAsync()
        {
            return Context.SaveChangesAsync();
        }

        public virtual Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Context.SaveChangesAsync(cancellationToken);
        }
        public void Update(T entity, bool saveNow = true)
        {
            ((DbContext)Context).Entry(entity).State = EntityState.Modified;
            if (saveNow)
            {
                Context.SaveChanges();
            }
        }

        public void UpdateRange(IEnumerable<T> entities, bool saveNow = true)
        {
            DbSet.UpdateRange(entities);
            if (saveNow)
            {
                SaveChanges();
            }
        }
        public async Task UpdateAsync(T entity, bool saveNow = true)
        {
            ((DbContext)Context).Entry(entity).State = EntityState.Modified;
            if (saveNow)
            {
                await Context.SaveChangesAsync();
            }
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities, bool updateNow = true)
        {
            DbSet.UpdateRange(entities);
            if (updateNow)
            {
                await SaveChangesAsync();
            }
        }

        public IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int? page = null, int? pageSize = null)
        {
            IQueryable<T> query = DbSet;
            if (orderBy != null)
                query = orderBy(query);
            if (predicate != null)
                query = query.Where(predicate);
            if (page == null || pageSize == null) return query;
            if (page <= 0) page = 1;

            if (pageSize <= 0) pageSize = 10;
            query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);


            return query;
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return Fetch(predicate).Count();
        }
        public IRepository<T> GetRepository<T>() where T : class
        {
            return _unitOfWork.GetRepository<T>();
        }
        public virtual IQueryable<T> Queryable()
        {
            return DbSet;
        }
        public virtual void Delete(T entity, bool saveNow = true)
        {
            ((DbContext)Context).Entry(entity).State = EntityState.Deleted;
            if (saveNow)
                Context.SaveChanges();
        }

        public void Delete(object entity, bool saveNow = true)
        {
            ((DbContext)Context).Entry(entity).State = EntityState.Deleted;
            if (saveNow)
                Context.SaveChanges();
        }
        public Task<bool> DeleteAsync(params object[] keyValues)
        {
            return DeleteAsync(CancellationToken.None, keyValues);
        }
        public virtual async Task<T> GetAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            return await DbSet.FindAsync(cancellationToken, keyValues);
        }
        public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = await GetAsync(cancellationToken, keyValues);

            if (entity == null)
                return false;
            ((DbContext)Context).Entry(entity).State = EntityState.Deleted;

            return true;
        }

        public void DeleteRange(IEnumerable<T> entity, bool saveNow = true)
        {
            DbSet.RemoveRange(entity);
            if (saveNow)
                SaveChanges();
        }
    }
}