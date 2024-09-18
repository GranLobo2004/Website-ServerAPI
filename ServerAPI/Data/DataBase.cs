using Microsoft.EntityFrameworkCore;
using ServerAPI.Entities;

namespace ServerAPI.Data;

public class DataBase: DbContext
{
    public DataBase(DbContextOptions<DataBase> options) : base(options) { }
    
    public DbSet<Product> Products { get; set; }
}