using Microsoft.EntityFrameworkCore;
using ServerAPI.Entities;

namespace ServerAPI.Data;

public class DataBase: DbContext
{
    public DataBase(DbContextOptions<DataBase> options) : base(options) { }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<User> Users { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlite("Data Source=database.db")
            .LogTo(Console.WriteLine, LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración del índice único para Email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }


}