namespace AcePacific.Common.Exceptions
{
    public class RepositoryNotFoundExceptions : Exception
    {
        public RepositoryNotFoundExceptions(string repositoryName, string message) : base(message)
        {
            if(string.IsNullOrEmpty(repositoryName))  throw new ArgumentNullException($"{nameof(repositoryName)}", nameof(repositoryName));
            this.RepositoryName = repositoryName;
        }
        public string RepositoryName { get; private set; }
    }
}
