using Microsoft.EntityFrameworkCore.Storage;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;
using plantita.Shared.Domain.Repositories;
using plantita.Shared.Infraestructure.Persistences.EFC.Configuration;

namespace plantita.Shared.Infraestructure.Persistences.EFC.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task CompleteAsync()
    {
        await _context.SaveChangesAsync();
    }
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }
}