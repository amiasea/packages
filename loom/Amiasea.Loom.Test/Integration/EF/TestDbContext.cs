using Microsoft.EntityFrameworkCore;
using Amiasea.Loom.Test.Integration.EF.Entities;

namespace Amiasea.Loom.Test.Integration.EF;

public sealed class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Post> Posts => Set<Post>();
}