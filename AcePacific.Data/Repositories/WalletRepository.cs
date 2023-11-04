using AcePacific.Common.Contract;
using AcePacific.Data.Entities;
using AcePacific.Data.RepositoryPattern;
using AcePacific.Data.RepositoryPattern.Implementations;
using AcePacific.Data.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AcePacific.Data.Repositories
{
    public interface IWalletRepository : IRepositoryAsync<Wallet>
    {
        IEnumerable<Wallet> GetWalletPaged(int page, int count, out int totalCount,
           WalletFilter filter = null, OrderExpression orderExpression = null);
        IEnumerable<Wallet> GetWalletPaged(int page, int count, WalletFilter filter = null,
            OrderExpression orderExpression = null);
        IEnumerable<Wallet> GetWalletFilteredQueryable(WalletFilter filter = null);
        Wallet GetWalletById(Guid id);
        Wallet GetWalletByReference(string reference);
    }
    public class WalletRepository : Repository<Wallet>, IWalletRepository
    {
        public WalletRepository(IDataContextAsync context, IUnitOfWork unitOfWork) : base(context, unitOfWork) { }

        public Wallet GetWalletById(Guid id)
        {
            var walletDetail = Table.AsNoTracking().FirstOrDefault(c => c.Id == id);
            return walletDetail;
        }
        public Wallet GetWalletByReference(string reference)
        {
            var walletDetail = Table.AsNoTracking().FirstOrDefault(c => c.TransactionReference == reference);
            return walletDetail;
        }
        public IEnumerable<Wallet> GetWalletFilteredQueryable(WalletFilter filter = null)
        {
            var expression = new WalletQueryObject(filter).Expression;
            return Fetch(expression); 
        }

        public IEnumerable<Wallet> GetWalletPaged(int page, int count, out int totalCount, WalletFilter filter = null, OrderExpression orderExpression = null)
        {
            var expression = new WalletQueryObject(filter).Expression;
            totalCount = Count(expression);
            return WalletPaged(page, count, expression, orderExpression);
        }

        private IEnumerable<Wallet> WalletPaged(int page, int count, Expression<Func<Wallet, bool>> expression, OrderExpression orderExpression)
        {
            var order = ProcessOrderFunc(orderExpression);
            return Fetch(expression, order, page, count);
        }
        public static Func<IQueryable<Wallet>, IOrderedQueryable<Wallet>> ProcessOrderFunc(OrderExpression orderDeserilizer = null)
        {
            Func<IQueryable<Wallet>, IOrderedQueryable<Wallet>> orderFuction = (queryable) =>
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
        public IEnumerable<Wallet> GetWalletPaged(int page, int count, WalletFilter filter = null, OrderExpression orderExpression = null)
        {
            var expression = new WalletQueryObject(filter).Expression;
            return WalletPaged(page, count, expression, orderExpression);
        }

        public class WalletQueryObject : QueryObject<Wallet>
        {
            public WalletQueryObject(WalletFilter filter)
            {
                if (filter != null)
                {

                    if (!string.IsNullOrEmpty(filter.TransactionReference))
                        And(c => c.TransactionReference.Contains(filter.TransactionReference));
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
