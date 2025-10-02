using Microsoft.EntityFrameworkCore.Storage;

namespace ChatApp.Application.Services.Contracts
{
    public interface ITransactionService
    {
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitAsync();
        Task RollBackAsync();
    }
}
