using Microsoft.EntityFrameworkCore;
using ProyectoConsolaObjetos1.Data;
using ProyectoConsolaObjetos1.Models;

namespace ProyectoConsolaObjetos1.UI;

public sealed class AppState
{
    public const int RolCliente = 1;
    public const int RolEmpleado = 2;

    public async Task<List<Usuario>> ObtenerUsuariosAsync(int? rol = null, string? filtro = null)
    {
        using var db = new WinformsDbContext();
        IQueryable<Usuario> query = db.Usuarios.AsNoTracking();
        if (rol.HasValue) query = query.Where(item => item.Rol == rol.Value);
        if (!string.IsNullOrWhiteSpace(filtro)) query = FiltrarUsuarios(query, filtro.Trim());
        return await query.OrderBy(item => item.Nombre).ToListAsync();
    }

    public async Task<Usuario> AgregarUsuarioAsync(Usuario usuario)
    {
        using var db = new WinformsDbContext();
        usuario.Id = 0;
        usuario.Codigo = await SiguienteCodigoUsuarioAsync(db);
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();
        return usuario;
    }

    public async Task<bool> ActualizarUsuarioAsync(Usuario usuario)
    {
        using var db = new WinformsDbContext();
        Usuario? existente = await db.Usuarios.FirstOrDefaultAsync(item => item.Id == usuario.Id);
        if (existente is null) return false;
        existente.Codigo = usuario.Codigo;
        existente.Nombre = usuario.Nombre;
        existente.Correo = usuario.Correo;
        existente.Clave = usuario.Clave;
        existente.Activo = usuario.Activo;
        existente.Rol = usuario.Rol;
        existente.Direccion = usuario.Direccion;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarUsuarioAsync(int id)
    {
        using var db = new WinformsDbContext();
        Usuario? usuario = await db.Usuarios.FirstOrDefaultAsync(item => item.Id == id);
        if (usuario is null) return false;
        db.Usuarios.Remove(usuario);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<List<Producto>> ObtenerProductosAsync(string? filtro = null)
    {
        using var db = new WinformsDbContext();
        IQueryable<Producto> query = db.Productos.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(filtro))
        {
            string criterio = filtro.Trim();
            query = query.Where(item => item.Codigo.ToString().Contains(criterio) || item.Nombre.Contains(criterio) || item.Categoria.Contains(criterio));
        }
        return await query.OrderBy(item => item.Nombre).ToListAsync();
    }

    public async Task<Producto> AgregarProductoAsync(Producto producto)
    {
        using var db = new WinformsDbContext();
        producto.Codigo = await SiguienteCodigoProductoAsync(db);
        db.Productos.Add(producto);
        await db.SaveChangesAsync();
        return producto;
    }

    public async Task<bool> ActualizarProductoAsync(Producto producto)
    {
        using var db = new WinformsDbContext();
        Producto? existente = await db.Productos.FirstOrDefaultAsync(item => item.Codigo == producto.Codigo);
        if (existente is null) return false;
        existente.Nombre = producto.Nombre;
        existente.Categoria = producto.Categoria;
        existente.Descripcion = producto.Descripcion;
        existente.PrecioVenta = producto.PrecioVenta;
        existente.StockActual = producto.StockActual;
        existente.StockMinimo = producto.StockMinimo;
        existente.Activo = producto.Activo;
        existente.Impuesto = producto.Impuesto;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarProductoAsync(int codigo)
    {
        using var db = new WinformsDbContext();
        Producto? producto = await db.Productos.FirstOrDefaultAsync(item => item.Codigo == codigo);
        if (producto is null) return false;
        db.Productos.Remove(producto);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<List<Venta>> ObtenerVentasAsync(string? filtro = null)
    {
        using var db = new WinformsDbContext();
        IQueryable<Venta> query = db.Ventas
            .AsNoTracking()
            .Include(item => item.Cliente)
            .Include(item => item.Empleado)
            .Include(item => item.Detallesventa)
            .ThenInclude(item => item.ProductoCodigoNavigation);
        if (!string.IsNullOrWhiteSpace(filtro))
        {
            string criterio = filtro.Trim();
            query = query.Where(item => item.Codigo.ToString().Contains(criterio) || item.Cliente.Nombre.Contains(criterio) || item.Empleado.Nombre.Contains(criterio));
        }
        return await query.OrderByDescending(item => item.FechaVenta).ToListAsync();
    }

    public async Task<Venta> AgregarVentaAsync(int clienteId, int empleadoId, IEnumerable<(int ProductoCodigo, int Cantidad)> detalles)
    {
        using var db = new WinformsDbContext();
        Venta venta = new() { Codigo = await SiguienteCodigoVentaAsync(db), ClienteId = clienteId, EmpleadoId = empleadoId, FechaVenta = DateTime.Now };
        foreach ((int productoCodigo, int cantidad) in detalles)
        {
            Producto? producto = await db.Productos.FirstOrDefaultAsync(item => item.Codigo == productoCodigo);
            if (producto is null) throw new InvalidOperationException($"No existe el producto {productoCodigo}.");
            venta.Detallesventa.Add(new Detallesventum { VentaCodigo = venta.Codigo, ProductoCodigo = producto.Codigo, Cantidad = cantidad, PrecioUnitario = producto.PrecioVenta, Impuesto = producto.Impuesto });
            producto.StockActual -= cantidad;
        }
        db.Ventas.Add(venta);
        await db.SaveChangesAsync();
        return venta;
    }

    public Task<Venta> CrearVentaAsync(int clienteId, int empleadoId, IEnumerable<(int ProductoCodigo, int Cantidad)> detalles) => AgregarVentaAsync(clienteId, empleadoId, detalles);

    public async Task<bool> EliminarVentaAsync(int codigo)
    {
        using var db = new WinformsDbContext();
        Venta? venta = await db.Ventas.FirstOrDefaultAsync(item => item.Codigo == codigo);
        if (venta is null) return false;
        db.Ventas.Remove(venta);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<int> SiguienteCodigoUsuarioAsync(int rol)
    {
        using var db = new WinformsDbContext();
        return await SiguienteCodigoUsuarioAsync(db, rol);
    }

    public async Task<int> SiguienteCodigoProductoAsync()
    {
        using var db = new WinformsDbContext();
        return await SiguienteCodigoProductoAsync(db);
    }

    public async Task<int> SiguienteCodigoVentaAsync()
    {
        using var db = new WinformsDbContext();
        return await SiguienteCodigoVentaAsync(db);
    }

    // Métodos para Categorías
public static async Task<List<Categoria>> ObtenerCategoriasAsync()
{
    using var db = new WinformsDbContext();
    return await db.Categorias.AsNoTracking().ToListAsync();
}

public static async Task AgregarCategoriaAsync(Categoria categoria)
{
    using var db = new WinformsDbContext();
    await db.Categorias.AddAsync(categoria);
    await db.SaveChangesAsync();
}

    private static IQueryable<Usuario> FiltrarUsuarios(IQueryable<Usuario> query, string criterio) => query.Where(item => item.Codigo.ToString().Contains(criterio) || item.Nombre.Contains(criterio) || item.Correo.Contains(criterio));
    private static async Task<int> SiguienteCodigoUsuarioAsync(WinformsDbContext db) => (await db.Usuarios.Select(item => (int?)item.Codigo).MaxAsync() ?? 0) + 1;
    private static async Task<int> SiguienteCodigoUsuarioAsync(WinformsDbContext db, int rol) => (await db.Usuarios.Where(item => item.Rol == rol).Select(item => (int?)item.Codigo).MaxAsync() ?? 0) + 1;
    private static async Task<int> SiguienteCodigoProductoAsync(WinformsDbContext db) => (await db.Productos.Select(item => (int?)item.Codigo).MaxAsync() ?? 0) + 1;
    private static async Task<int> SiguienteCodigoVentaAsync(WinformsDbContext db) => (await db.Ventas.Select(item => (int?)item.Codigo).MaxAsync() ?? 0) + 1;
}
