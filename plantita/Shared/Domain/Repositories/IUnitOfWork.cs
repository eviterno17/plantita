using Microsoft.EntityFrameworkCore.Storage;

namespace plantita.Shared.Domain.Repositories;

public interface IUnitOfWork
{

    Task CompleteAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();

}