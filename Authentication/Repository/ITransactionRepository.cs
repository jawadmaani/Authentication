using Microsoft.EntityFrameworkCore.Storage;

namespace Authentication.Repository;

public interface ITransactionRepository
{
     Task<IDbContextTransaction> BeginTransactionAsync();
     Task CommitTransactionAsync();
     Task RollbackTransactionAsync();
}