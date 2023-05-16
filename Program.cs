using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ProductoDb>(opt => opt.UseInMemoryDatabase("ProductoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

//Con la siguiente url pueden acceder a la pagina, por pagina se obtiene 5 productos http://localhost:DEPENDIENDO SU PUERTO/productos?page=2&pageSize=5 By Stephanie uwu
/*
Esto fue una prueba que hice creando una lista para el get y aparezca paginado 
var productos = new List<Producto>
{
    new Producto { Id = 1, Nombre = "Chips Moradas", Precio = 17 },
    new Producto { Id = 2, Nombre = "Chips Verdes", Precio = 17},
    new Producto { Id = 3, Nombre = "Ruffles Verdes", Precio = 30.50 },
    new Producto { Id = 4, Nombre = "Ruffles Azules", Precio = 22.50 },
    new Producto { Id = 5, Nombre = "Carlos V chocolate", Precio = 10 },
    new Producto { Id = 6, Nombre = "Kinder Sorpresa Chocolate", Precio = 6 },
    new Producto { Id = 7, Nombre = "Snickers", Precio = 30 },
    new Producto { Id = 8, Nombre = "Sopa Nissin C", Precio = 18.50 },
    new Producto { Id = 9, Nombre = "Sopa Nissin P", Precio = 18.50 },
    new Producto { Id = 10, Nombre = "Galletas Maria", Precio = 12 },
};


//Así fue como inicie xd 
app.MapGet("/productos", async (ProductoDb db) =>
    await db.Productos.ToListAsync());

*/
app.MapGet("/productos", async (ProductoDb db, int page = 1, int pageSize = 5) =>
{
    var skip = (page - 1) * pageSize;
    var take = pageSize;

    var items = await db.Productos.Skip(skip).Take(take).ToListAsync();

    var totalCount = await db.Productos.CountAsync();
    var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

    var response = new
    {
        Items = items,
        Page = page,
        PageSize = pageSize,
        TotalCount = totalCount,
        TotalPages = totalPages
    };

    return Results.Ok(response);
});

app.MapGet("/productos/{id}", async (int id, ProductoDb db) =>
    await db.Productos.FindAsync(id)
        is Producto producto
            ? Results.Ok(producto)
            : Results.NotFound());

app.MapPost("/productos", async (Producto producto, ProductoDb db) =>
{
    // Agregar el producto a la base de datos
    db.Productos.Add(producto);
    await db.SaveChangesAsync();

    // Retornar el producto agregado
    return Results.Created($"/productos/{producto.Id}", producto);
});



app.MapPut("/productos/{id}", async (int id, Producto inputProducto, ProductoDb db) => {
    var producto = await db.Productos.FindAsync(id);
    if (producto is null) return Results.NotFound();

    producto.Nombre = inputProducto.Nombre;
    producto.Precio = inputProducto.Precio;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/productos/{id}", async (int id, ProductoDb db) => {
    if (await db.Productos.FindAsync(id) is Producto producto)
    {
        db.Productos.Remove(producto);
        await db.SaveChangesAsync();
        return Results.Ok(producto);
    }
    return Results.NotFound();
});

app.Run();