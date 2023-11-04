namespace AcePacific.Data.RepositoryPattern
{
    public interface IDataContext : IDisposable
    {
        int SaveChanges();
    }
}
