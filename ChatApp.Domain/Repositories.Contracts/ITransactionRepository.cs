using Microsoft.EntityFrameworkCore.Storage;

namespace ChatApp.Domain.Repositories.Contracts
{
    public interface ITransactionRepository
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitAsync();
        Task RollBackAsync();
    }
}
