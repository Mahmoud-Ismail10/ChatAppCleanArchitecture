﻿namespace ChatApp.Domain.Repositories.Contracts
{
    public interface IGenericRepositoryAsync<T> where T : class
    {
        Task DeleteRangeAsync(ICollection<T> entities);
        Task<T?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<T?>> GetAllAsync();
        Task SaveChangesAsync();
        IQueryable<T> GetTableNoTracking();
        IQueryable<T> GetTableAsTracking();
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(ICollection<T> entities);
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(ICollection<T> entities);
        Task DeleteAsync(T entity);
        void AttachEntity<TEntity>(TEntity entity) where TEntity : class;
    }
}
