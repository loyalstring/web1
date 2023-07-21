using Microsoft.EntityFrameworkCore;
using JewelleryWebApplication.Data;
using JewelleryWebApplication.Base.Interface;
using JewelleryWebApplication.Base.Model;
using System.Linq.Expressions;

namespace JewelleryWebApplication.Base.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseModel
    {
        private readonly JewelleryWebApplicationContext _context;
        private readonly DbSet<T> entities;
        public BaseRepository(JewelleryWebApplicationContext context)
        {
            _context = context;
            entities = _context.Set<T>();
        }
        public async Task<List<T>> AllAsync()
        {
            return await entities.Where(x => x.StatusType).ToListAsync();
        }

        public async Task<List<T>> AllAsync(params Expression<Func<T, Object>>[] includeProperties)
        {
            IQueryable<T> queryable = entities.Where(x => x.StatusType);
            foreach (Expression<Func<T, object>> includeProperty in includeProperties)
            {
                queryable = queryable.Include<T, object>(includeProperty);
            }
            return await queryable.ToListAsync();
        }

        public async Task<int> DeleteAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Remove(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async void DeleteAllAsync(IEnumerable<T> e)
        {
            entities.RemoveRange(e);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await entities.AnyAsync(s => s.Id == id);
        }
        public async Task<T> FindAsync(int id)
        {
            return await entities.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<T> FindAsync(int id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> queryable = entities.Where(x => x.StatusType);
            foreach (Expression<Func<T, object>> includeProperty in includeProperties)
            {

                queryable = queryable.Include<T, object>(includeProperty);
            }
            List<T> data = await queryable.ToListAsync();
            return data.FirstOrDefault(s => s.Id == id);
        }
        
        public async Task<T> InsertAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entity.CreatedOn = DateTime.UtcNow;
            entity.LastUpdated = DateTime.UtcNow;
            entity.StatusType = true;
            await entities.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task BulkInsertAsync(IEnumerable<T> e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("entity");
            }

            var list = e.ToList();
            list.ForEach(d =>
            {
                d.CreatedOn = DateTime.UtcNow;
                d.LastUpdated = DateTime.UtcNow;
                d.StatusType=true;
            });
            await entities.AddRangeAsync(list);
            await _context.SaveChangesAsync();
        }
       
        public async Task<T> UpdateAsync(T entity, int id)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            T exist = _context.Set<T>().Find(id);
            if (exist != null)
            {
                entity.LastUpdated = DateTime.UtcNow;
                _context.Entry(exist).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
            }
            await _context.SaveChangesAsync();
            return entity;
        }
        //public async void FromSql(string query)
        //{
        //    await _context.Database.ExecuteSqlRawAsync(query);
        //}
        public async Task FromSqlAsync(string query)
        {
            await _context.Database.ExecuteSqlRawAsync(query);
        }
        public IQueryable<T> All(params Expression<Func<T, Object>>[] includeProperties)
        {
            IQueryable<T> queryable = entities.Where(x =>x.StatusType);
            foreach (Expression<Func<T, object>> includeProperty in includeProperties)
            {
                queryable = queryable.Include<T, object>(includeProperty);
            }
            return queryable;
        }
        public IQueryable<T> All()
        {
            return entities.Where(x => x.StatusType);
        }
        public T Find(int id)
        {
            return entities.SingleOrDefault(s => s.Id == id);
        }

    }
}
