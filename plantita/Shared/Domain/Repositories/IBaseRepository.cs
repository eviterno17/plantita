namespace plantita.Shared.Domain.Repositories;

public interface IBaseRepository<TEntity>
{
    Task AddAsync(TEntity entity);

    Task<TEntity?> FindByIdAsync(int id);
    Task<TEntity?> FindByIdGuidAsync(Guid id);

    Task UpdateAsync(TEntity entity);
    Task AddSync(TEntity entity);

    Task DeleteAsync(int id);

    void Update(TEntity entity);

    void Remove(TEntity entity);

    Task<IEnumerable<TEntity>> ListAsync();
}