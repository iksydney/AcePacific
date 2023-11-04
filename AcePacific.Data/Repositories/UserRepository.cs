using AcePacific.Common.Contract;
using AcePacific.Data.Entities;
using AcePacific.Data.RepositoryPattern;
using AcePacific.Data.RepositoryPattern.Implementations;
using AcePacific.Data.ViewModel;
using System.Data.Entity;
using System.Linq.Expressions;

namespace AcePacific.Data.Repositories
{
    public interface IUserRepository : IRepositoryAsync<User>
    {
        IEnumerable<User> GetCustomerPaged(int page, int count, out int totalCount,
           CustomerFilter filter = null, OrderExpression orderExpression = null);
        IEnumerable<User> GetCustomerPaged(int page, int count, CustomerFilter filter = null,
            OrderExpression orderExpression = null);
        IEnumerable<User> GetCustomerFilteredQueryable(CustomerFilter filter = null);
        bool UserNameExists(string userName);
        bool EmailExiststs(string email);
        bool PhoneNumberExists(string phoneNumber);
        User FindUserById(string id);
        User FindUserByName(string userName);
        User FindUserByEmail(string email);
        User FindByAccountNumber(string accountNumber);
        User FindByPhoneNumber(string phoneNumber);
        bool AccountNumberExists(string accountNumber);
    }
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IDataContextAsync context, IUnitOfWork unitOfWork) : base(context, unitOfWork) { }

        public bool EmailExiststs(string email)
        {
            return Table.AsNoTracking().Any(c => c.Email == email);
        }
        public bool AccountNumberExists(string accountNumber)
        {
            return Table.AsNoTracking().Any(c =>c.AccountNumber == accountNumber);
        }

        public User FindUserByEmail(string email)
        {
            var entity = Table.AsNoTracking().FirstOrDefault(c => c.Email == email);
            return entity;
        }

        public User FindUserById(string id)
        {
            var entity = Table.AsNoTracking().FirstOrDefault(c => c.Id == id);
            return entity;

        }
        public User FindByPhoneNumber(string phoneNumber)
        {
            return Table.AsNoTracking().FirstOrDefault(c => c.PhoneNumber == phoneNumber);
        }
        public User FindByAccountNumber(string accountNumber)
        {
            return Table.AsNoTracking().FirstOrDefault(c => c.AccountNumber == accountNumber);
        }

        public User FindUserByName(string userName)
        {
            var entity = Table.AsNoTracking().FirstOrDefault(c => c.UserName == userName);
            return entity;
        }

        public bool PhoneNumberExists(string phoneNumber)
        {
            return Table.AsNoTracking().Any(c => c.PhoneNumber == phoneNumber);
        }

        public bool UserNameExists(string userName)
        {
            return Table.AsNoTracking().Any(c => c.UserName != userName);
        }

        public IEnumerable<User> GetCustomerPaged(int page, int count, out int totalCount, CustomerFilter filter = null, OrderExpression orderExpression = null)
        {
            var expression = new CustomerQueryObject(filter).Expression;
            totalCount = Count(expression);
            return CustomerPaged(page, count, expression, orderExpression);
        }

        private IEnumerable<User> CustomerPaged(int page, int count, Expression<Func<User, bool>> expression, OrderExpression orderExpression)
        {
            var order = ProcessOrderFunc(orderExpression);
            return Fetch(expression, order, page, count);
        }

        public IEnumerable<User> GetCustomerPaged(int page, int count, CustomerFilter filter = null, OrderExpression orderExpression = null)
        {
            var expression = new CustomerQueryObject(filter).Expression;
            return CustomerPaged(page, count, expression, orderExpression);
        }

        public IEnumerable<User> GetCustomerFilteredQueryable(CustomerFilter filter = null)
        {
            var expression = new CustomerQueryObject(filter).Expression;
            return Fetch(expression);
        }
        public static Func<IQueryable<User>, IOrderedQueryable<User>> ProcessOrderFunc(OrderExpression orderDeserilizer = null)
        {
            Func<IQueryable<User>, IOrderedQueryable<User>> orderFuction = (queryable) =>
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
        public class CustomerQueryObject : QueryObject<User>
        {
            public CustomerQueryObject(CustomerFilter filter)
            {
                if (filter != null)
                {

                    if (!string.IsNullOrEmpty(filter.PhoneNumber))
                        And(c => c.PhoneNumber.Contains(filter.PhoneNumber));
                    if (!string.IsNullOrEmpty(filter.UserName))
                        And(c => c.UserName.ToLower().Contains(filter.UserName.ToLower()));
                    if (!string.IsNullOrEmpty(filter.Name))
                        And(c => c.FirstName.ToLower().Contains(filter.Name.ToLower()) || c.LastName.ToLower().Contains(filter.Name.ToLower()));
                    if (!string.IsNullOrEmpty(filter.FirstName))
                    {
                        And(c => c.FirstName.ToLower().Contains(filter.FirstName.ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filter.LastName))
                        And(c => c.LastName.ToLower().Contains(filter.LastName.ToLower()));
                    if (!string.IsNullOrEmpty(filter.Email))
                        And(c => c.Email.ToLower().Contains(filter.Email.ToLower()));
                    if (filter.DateCreatedFrom.HasValue)
                        And(c => c.DateCreated >= filter.DateCreatedFrom);
                    if (filter.DateCreatedTo.HasValue)
                        And(c => c.DateCreated < filter.DateCreatedTo.Value.AddDays(1));
                }
            }
        }
    }
}
