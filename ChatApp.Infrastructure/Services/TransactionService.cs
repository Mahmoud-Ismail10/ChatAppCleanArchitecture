using ChatApp.Application.Services.Contracts;
using ChatApp.Domain.Repositories.Contracts;
using Microsoft.EntityFrameworkCore.Storage;

namespace ChatApp.Infrastructure.Services
{
    public class TransactionService : ITransactionService
    {
        #region Fields
        private readonly ITransactionRepository _transactionRepository;
        #endregion

        #region Constructors
        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
        #endregion

        #region Functions
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _transactionRepository.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _transactionRepository.CommitAsync();
        }

        public async Task RollBackAsync()
        {
            await _transactionRepository.RollBackAsync();
        }
        #endregion
    }
}
