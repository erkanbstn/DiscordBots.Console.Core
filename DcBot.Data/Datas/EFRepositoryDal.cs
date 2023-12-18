using DcBot.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DcBot.Data.Datas
{
    public class EFRepositoryDal<T> : IRepositoryDal<T> where T : class
    {
        private readonly AppDbContext _appDbContext;
        DbSet<T> _object;
        public EFRepositoryDal(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _object = _appDbContext.Set<T>();
        }

        public async Task ChangeStatusAllAsync(List<T> t)
        {
            _object.UpdateRange(t);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task ChangeStatusAsync(T t)
        {
            _object.Update(t);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T t)
        {
            _object.Remove(t);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter)
        {
            return await _object.FirstOrDefaultAsync(filter);
        }

        public async Task<T> GetByDiscordIdAsync(Expression<Func<T, bool>> filter)
        {
            return await _object.FirstOrDefaultAsync(filter);
        }

        public async Task<T> GetByIdAsync(int? id)
        {
            return await _object.FindAsync(id);
        }

        public async Task InsertAsync(T t)
        {
            await _object.AddAsync(t);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<List<T>> ToListAsync()
        {
            return await _object.AsNoTracking().ToListAsync();
        }

        public async Task<List<T>> ToListByFilterAsync(Expression<Func<T, bool>> filter)
        {
            return await _object.Where(filter).AsNoTracking().ToListAsync();
        }

        public async Task UpdateAsync(T t)
        {
            _object.Update(t);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
