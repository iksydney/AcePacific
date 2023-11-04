using System.Linq.Expressions;

namespace AcePacific.Data.RepositoryPattern
{
    public interface IDataRepository
    {
        /// <summary>
        /// Get all Elements of Type T
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <returns>DbSet of Entities</returns>
        IQueryable<T> Query<T>() where T : class;

        IQueryable<T> Query<T>(Expression<Func<T, bool>> predicate) where T : class;

        IQueryable<T> AsNoTrackingQuery<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// placeholder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        IQueryable<T> Query<T>(params Expression<Func<T, object>>[] includeProperties) where T : class;

        /// <summary>
        /// Add an Entity to the Persistence Storage
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="item">Entity to be Added</param>
        void Add<T>(T item) where T : class;

        /// <summary>
        /// Delete an Entity from the Persistence Storage
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="item">Entity to be Removed</param>
        void Delete<T>(T item) where T : class;

        /// <summary>
        /// Get a Single Entity from the Persistence Storage using the Entities' ID
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="id">ID of the Entity</param>
        /// <returns>Single Entity or Null</returns>
        T GetByID<T>(int id) where T : class;

        /// <summary>
        /// Get a Single Entity from the Persistence Storage using the Entities' ID
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="id">ID of the Entity</param>
        /// <returns>Single Entity or Null</returns>
        T GetByID<T>(long id) where T : class;

        /// <summary>
        /// Get a Single Entity from the Persistence Storage using the Entities' ID
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="id">ID of the Entity</param>
        /// <returns>Single Entity or Null</returns>
        T GetByID<T>(string id) where T : class;

        /// <summary>
        /// Updates a Specific Entity to the Persistence Storage
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        void Update<T>(T item) where T : class;

        /// <summary>
        /// Execute the store procedure and accept objects eg new{ id = 1};
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sprocname"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        IEnumerable<T> Execute<T>(string sprocname, object args) where T : class;
        /// <summary>
        ///  Execute sql statement  ...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        IEnumerable<T> Execute<T>(string sql) where T : class;

        /// <summary>
        /// Execute  sql and passing args @ paramters ..
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        void Execute(string sql, object args);
        /// <summary>
        /// Execute sql statement ...
        /// </summary>
        /// <param name="sql"></param>
        void Execute(string sql);
    }
}
