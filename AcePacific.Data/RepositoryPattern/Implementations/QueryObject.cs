using LinqKit;
using System.Linq.Expressions;

namespace AcePacific.Data.RepositoryPattern.Implementations
{
    public abstract class QueryObject<TEntity> : IQueryObject<TEntity>
    {
        private Expression<Func<TEntity, bool>> _query;

        public virtual Expression<Func<TEntity, bool>> Expression => _query;

        public IQueryObject<TEntity> And(Expression<Func<TEntity, bool>> query)
        {
            _query = _query == null ? query : _query.And(query.Expand());
            return this;

        }

        public IQueryObject<TEntity> And(IQueryObject<TEntity> queryObject)
        {
            return And(queryObject.Expression);
        }

        public IQueryObject<TEntity> Or(Expression<Func<TEntity, bool>> query)
        {
            _query = _query == null ? query : _query.Or(query.Expand());
            return this;
        }

        public IQueryObject<TEntity> Or(IQueryObject<TEntity> queryObject)
        {
            return Or(queryObject.Expression);
        }
    }
}
