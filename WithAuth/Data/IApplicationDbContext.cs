using Microsoft.EntityFrameworkCore;
using WithAuth.Models;

namespace WithAuth.Data;

public interface IApplicationDbContext
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}