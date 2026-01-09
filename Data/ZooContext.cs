using Microsoft.EntityFrameworkCore;
using DierentuinApp.Models;

namespace DierentuinApp.Data;

// SQLite wordt gebruikt i.p.v. SQL Server, omdat in het inleverpunt van brightspace staat dat we SQLite moeten gebruiken.
public class ZooContext : DbContext
{
    public ZooContext(DbContextOptions<ZooContext> options) : base(options) { }

    public DbSet<Animal> Animals { get; set; }
    public DbSet<Enclosure> Enclosures { get; set; }
    public DbSet<Category> Categories { get; set; }
}