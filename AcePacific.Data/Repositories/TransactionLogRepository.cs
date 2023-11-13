using AcePacific.Common.Contract;
using AcePacific.Data.Entities;
using AcePacific.Data.RepositoryPattern;
using AcePacific.Data.RepositoryPattern.Implementations;
using AcePacific.Data.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AcePacific.Data.Repositories
{
    public interface ITransactionLogRepository : IRepositoryAsync<TransactionLog>
    {
        IEnumerable<TransactionLog> GetTransactionLogPaged(int page, int count, out int totalCount,
           TransactionLogFilter filter = null, OrderExpression orderExpression = null);
        IEnumerable<TransactionLog> GetTransactionLogPaged(int page, int count, TransactionLogFilter filter = null,
            OrderExpression orderExpression = null);
        IEnumerable<TransactionLog> GetTransactionLogFilteredQueryable(TransactionLogFilter filter = null);
        TransactionLog GetlogById(int id);
        TransactionLog GetTransactionLogByReference(string transactionReference);
    }
    public class TransactionLogRepository : Repository<TransactionLog>, ITransactionLogRepository
    {
        public TransactionLogRepository(IDataContextAsync context, IUnitOfWork unitOfWork) : base(context, unitOfWork) { }

        public TransactionLog GetlogById(int id)
        {
            var transactionDetail = Table.AsNoTracking().FirstOrDefault(c => c.Id == id);
            return transactionDetail;
        }
        public TransactionLog GetTransactionLogByReference(string transactionReference)
        {
            var transactionDetail = Table.AsNoTracking().FirstOrDefault(c => c.Reference == transactionReference);
            return transactionDetail;
        }
        public IEnumerable<TransactionLog> GetTransactionLogFilteredQueryable(TransactionLogFilter filter = null)
        {
            var expression = new TransactionLogQueryObject(filter).Expression;
            return Fetch(expression); 
        }

        public IEnumerable<TransactionLog> GetTransactionLogPaged(int page, int count, out int totalCount, TransactionLogFilter filter = null, OrderExpression orderExpression = null)
        {
            var expression = new TransactionLogQueryObject(filter).Expression;
            totalCount = Count(expression);
            return TransactionLogPaged(page, count, expression, orderExpression);
        }

        private IEnumerable<TransactionLog> TransactionLogPaged(int page, int count, Expression<Func<TransactionLog, bool>> expression, OrderExpression orderExpression)
        {
            var order = ProcessOrderFunc(orderExpression);
            return Fetch(expression, order, page, count);
        }
        public static Func<IQueryable<TransactionLog>, IOrderedQueryable<TransactionLog>> ProcessOrderFunc(OrderExpression orderDeserilizer = null)
        {
            Func<IQueryable<TransactionLog>, IOrderedQueryable<TransactionLog>> orderFuction = (queryable) =>
            {
                var orderQueryable = queryable.OrderByDescending(x => x.Id).ThenBy(c => c.DateCreated);
                if (orderDeserilizer != null)
                {
                    switch (orderDeserilizer.Column)
                    {

                    }
                }
                return orderQueryable;
            };
            return orderFuction;
        }
        public IEnumerable<TransactionLog> GetTransactionLogPaged(int page, int count, TransactionLogFilter filter = null, OrderExpression orderExpression = null)
        {
            var expression = new TransactionLogQueryObject(filter).Expression;
            return TransactionLogPaged(page, count, expression, orderExpression);
        }

        public class TransactionLogQueryObject : QueryObject<TransactionLog>
        {
            public TransactionLogQueryObject(TransactionLogFilter filter)
            {
                if (filter != null)
                {

                    if (!string.IsNullOrEmpty(filter.Reference))
                        And(c => c.Reference.Contains(filter.Reference));
                    if (!string.IsNullOrEmpty(filter.UserId))
                        And(c => c.UserId.ToLower().Contains(filter.UserId.ToLower()));
                    if (filter.DateCreatedFrom.HasValue)
                        And(c => c.DateCreated >= filter.DateCreatedFrom);
                    if (filter.DateCreatedTo.HasValue)
                        And(c => c.DateCreated < filter.DateCreatedTo.Value.AddDays(1));
                }
            }
        }
    }
}
