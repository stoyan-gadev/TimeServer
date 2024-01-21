using Microsoft.EntityFrameworkCore;
using TimeServer.DataAccess.Entities;

namespace TimeServer.DataAccess;

public class ApplicationDbContext : DbContext
{
    public DbSet<Log> Logs => Set<Log>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}
