using JewelleryWebApplication.Base;
using JewelleryWebApplication.Base.Model;
using System.Linq.Expressions;

namespace JewelleryWebApplication.Base.Interface
{
    public interface IBaseRepository<T> where T : BaseModel
    {
        IQueryable<T> All();
        IQueryable<T> All(params Expression<Func<T, Object>>[] includeProperties);
        T Find(int id);
        Task<List<T>> AllAsync();
        Task<List<T>> AllAsync(params Expression<Func<T, Object>>[] includeProperties);
        Task<T> FindAsync(int id);
        Task<T> FindAsync(int id, params Expression<Func<T, object>>[] includeProperties);
        Task<T> InsertAsync(T entity);
        Task BulkInsertAsync(IEnumerable<T> e);
      
        Task FromSqlAsync(string query);
        Task<T> UpdateAsync(T entity, int id);
        Task<int> DeleteAsync(T entity);
        void DeleteAllAsync(IEnumerable<T> e);
        Task<bool> ExistsAsync(int id);
    }
}
