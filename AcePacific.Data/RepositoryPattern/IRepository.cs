using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AcePacific.Data.RepositoryPattern
{
    public interface IRepository<T> where T : class
    {
        T Find(long id);
        T Find(string id);
        T Find(int id);
        T Find(Guid id);
        IEnumerable<T> GetAll();
        IQueryable<T> Table { get; }
        T Find(Expression<Func<T, bool>> predicate);
        void insert(T entity, bool saveNow = true);
        void insertRange(IEnumerable<T> entities, bool saveNow = true);
        IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null,
            int? page = null, int? pageSize = null);
        int Count(Expression<Func<T, bool>> predicate);
        void Update(T entity, bool saveNow = true);
        void UpdateRange(IEnumerable<T> entities, bool saveNow = true);
        void Delete(T entity, bool saveNow = true);
        void Delete(object entity, bool saveNow = true);
        void DeleteRange(IEnumerable<T> entity, bool saveNow = true);
        IRepository<T> GetRepository<T>() where T : class;
    }
}
