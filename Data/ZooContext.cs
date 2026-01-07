using Microsoft.EntityFrameworkCore;
using DierentuinApp.Models;

namespace DierentuinApp.Data;

public class ZooContext : DbContext
{
    public ZooContext(DbContextOptions<ZooContext> options) : base(options) { }

    public DbSet<Animal> Animals { get; set; }
    public DbSet<Enclosure> Enclosures { get; set; }
}