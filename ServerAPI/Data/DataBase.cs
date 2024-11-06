// DataBase.cs
using Microsoft.EntityFrameworkCore;
using ServerAPI.Entities;

namespace ServerAPI.Data
{
    public class DataBase : DbContext
    {
        public DataBase(DbContextOptions<DataBase> options) : base(options) { }
        
        public DbSet<Product> Products { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<ProductPurchaseUser> ProductPurchaseUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite("Data Source=database.db")
                .LogTo(Console.WriteLine, LogLevel.Information);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Comment>()
                .HasKey(c => new{c.Id, c.ProductId});

            modelBuilder.Entity<Purchase>()
                .HasKey(o => o.Id); // Clave primaria

            modelBuilder.Entity<ProductPurchaseUser>()
                .HasKey(po => new { po.PurchaseId, po.ProductId, po.UserId }); // Clave primaria compuesta

            modelBuilder.Entity<ProductPurchaseUser>()
                .HasOne<Purchase>()
                .WithMany() // Relación con Order
                .HasForeignKey(po => po.PurchaseId);

            modelBuilder.Entity<ProductPurchaseUser>()
                .HasOne<Product>()
                .WithMany() // Relación con Product
                .HasForeignKey(po => po.ProductId);

            modelBuilder.Entity<ProductPurchaseUser>()
                .HasOne<User>() // Relación con User
                .WithMany() // Relación con User
                .HasForeignKey(po => po.UserId);
        }
    }
}