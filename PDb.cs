using Microsoft.EntityFrameworkCore;

class ProductoDb : DbContext
{
    public ProductoDb(DbContextOptions<ProductoDb> options)
        : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
}