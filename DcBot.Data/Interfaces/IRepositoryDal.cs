using System.Linq.Expressions;

namespace DcBot.Data.Interfaces
{
    public interface IRepositoryDal<T>
    {
        Task InsertAsync(T t);
        Task DeleteAsync(T t);
        Task ChangeStatusAsync(T t);
        Task UpdateAsync(T t);
        Task<T> GetByIdAsync(int? id);
        Task<T> GetByDiscordIdAsync(Expression<Func<T, bool>> filter);
        Task<List<T>> ToListAsync();
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter);
        Task<List<T>> ToListByFilterAsync(Expression<Func<T, bool>> filter);
    }
}