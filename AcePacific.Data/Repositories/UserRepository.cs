using AcePacific.Common.Contract;
using AcePacific.Data.Entities;
using AcePacific.Data.RepositoryPattern;
using AcePacific.Data.RepositoryPattern.Implementations;
using AcePacific.Data.ViewModel;
using System.Data.Entity;
using System.Linq.Expressions;

namespace AcePacific.Data.Repositories
{
    public interface IBankRepository : IRepositoryAsync<Bank>
    {
        IEnumerable<Bank> GetBankPaged(int page, int count, out int totalCount,
           BankFilter filter = null, OrderExpression orderExpression = null);
        IEnumerable<Bank> GetBankPaged(int page, int count, BankFilter filter = null,
            OrderExpression orderExpression = null);
        IEnumerable<Bank> GetBankFilteredQueryable(BankFilter filter = null);
        bool BankExists(string bankName);
        bool bankCodeExiststs(string bankCode);
        Bank FindUserByBankId(int id);
        Bank FindUserByBankName(string bankName);
        Bank FindByBankCode(string bankCode);
    }
    public class BankRepository : Repository<Bank>, IBankRepository
    {
        public BankRepository(IDataContextAsync context, IUnitOfWork unitOfWork) : base(context, unitOfWork) { }

        public bool BankExists(string bankName)
        {
            return Table.AsNoTracking().Any(c => c.BankName == bankName);
        }
        public bool bankCodeExiststs(string bankCode)
        {
            return Table.AsNoTracking().Any(c =>c.BankCode == bankCode);
        }

        public Bank FindUserByBankId(int id)
        {
            var entity = Table.AsNoTracking().FirstOrDefault(c => c.Id == id);
            return entity;
        }

        public Bank FindUserByBankName(string bankName)
        {
            var entity = Table.AsNoTracking().FirstOrDefault(c => c.BankName == bankName);
            return entity;
        }
        public Bank FindByBankCode(string bankCode)
        {
            var entity = Table.AsNoTracking().FirstOrDefault(c => c.BankCode == bankCode);
            return entity;
        }
        

        public IEnumerable<Bank> GetBankPaged(int page, int count, out int totalCount, BankFilter filter = null, OrderExpression orderExpression = null)
        {
            var expression = new BankQueryObject(filter).Expression;
            totalCount = Count(expression);
            return BankPaged(page, count, expression, orderExpression);
        }

        private IEnumerable<Bank> BankPaged(int page, int count, Expression<Func<Bank, bool>> expression, OrderExpression orderExpression)
        {
            var order = ProcessOrderFunc(orderExpression);
            return Fetch(expression, order, page, count);
        }

        public IEnumerable<Bank> GetBankPaged(int page, int count, BankFilter filter = null, OrderExpression orderExpression = null)
        {
            var expression = new BankQueryObject(filter).Expression;
            return BankPaged(page, count, expression, orderExpression);
        }

        public IEnumerable<Bank> GetBankFilteredQueryable(BankFilter filter = null)
        {
            var expression = new BankQueryObject(filter).Expression;
            return Fetch(expression);
        }
        public static Func<IQueryable<Bank>, IOrderedQueryable<Bank>> ProcessOrderFunc(OrderExpression orderDeserilizer = null)
        {
            Func<IQueryable<Bank>, IOrderedQueryable<Bank>> orderFuction = (queryable) =>
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
        public class BankQueryObject : QueryObject<Bank>
        {
            public BankQueryObject(BankFilter filter)
            {
                if (filter != null)
                {

                    if (!string.IsNullOrEmpty(filter.BankName))
                        And(c => c.BankName.Contains(filter.BankName));
                    if (!string.IsNullOrEmpty(filter.BankCode))
                        And(c => c.BankCode.ToLower().Contains(filter.BankCode.ToLower()));
                    if (filter.DateCreatedFrom.HasValue)
                        And(c => c.DateCreated >= filter.DateCreatedFrom);
                    if (filter.DateCreatedTo.HasValue)
                        And(c => c.DateCreated < filter.DateCreatedTo.Value.AddDays(1));
                }
            }
        }
    }
}
