using AcePacific.Data.Entities;
using AcePacific.Data.RepositoryPattern;
using AcePacific.Data.RepositoryPattern.Implementations;

namespace AcePacific.Data.Repositories
{
    public interface IAdminPendingTransactionsRepository : IRepositoryAsync<AdminPendingTransactions>
    {
    }
    public class AdminPendingTransactionsRepository : Repository<AdminPendingTransactions>, IAdminPendingTransactionsRepository
    {
        public AdminPendingTransactionsRepository(IDataContextAsync context, IUnitOfWork unitOfWork) : base(context, unitOfWork) { }
    }
}
