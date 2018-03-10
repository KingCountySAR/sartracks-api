using Microsoft.EntityFrameworkCore;
using SarData.Api.Client;

namespace SarData.Api.Data
{
  public class StoreContext : DbContext
  {
    public StoreContext(DbContextOptions<StoreContext> options) : base(options)
    {
    }

    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Jurisdiction> Jurisdictions { get; set; }
  }
}
