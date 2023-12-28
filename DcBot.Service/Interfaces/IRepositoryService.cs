using System.Linq.Expressions;

namespace DcBot.Service.Interfaces
{
    public interface IRepositoryService<T>
    {
        Task DeleteAllQueryAsync(string tableName);
        Task<List<T>> ToListByNoTrackAsync();
        Task<List<T>> ToListFilteringByNoTrackAsync(Expression<Func<T, bool>> filter);
        Task InsertAsync(T t);
        Task DeleteAsync(T t);
        Task ChangeStatusAsync(T t, bool status);
        Task UpdateAsync(T t);
        Task<T> GetByIdAsync(int? id);
        Task<T> GetByDiscordIdAsync(ulong id, int? dcServerId);
        Task<List<T>> ToListAsync();
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter);
        Task<List<T>> ToListByFilterAsync(Expression<Func<T, bool>> filter);
    }
}