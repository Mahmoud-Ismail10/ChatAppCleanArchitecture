using ChatApp.Domain.Repositories.Contracts;
using ChatApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace ChatApp.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        #region Fields
        protected readonly ChatAppDbContext _dbContext;
        #endregion

        #region Constructors
        public TransactionRepository(ChatAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        #endregion

        #region Functions
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _dbContext.Database.CommitTransactionAsync();
        }

        public async Task RollBackAsync()
        {
            await _dbContext.Database.RollbackTransactionAsync();
        }
        #endregion
    }
}
