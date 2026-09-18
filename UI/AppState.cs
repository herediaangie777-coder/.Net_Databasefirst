using Microsoft.EntityFrameworkCore;
using ProyectoConsolaObjetos1.Data;
using ProyectoConsolaObjetos1.Models;

namespace ProyectoConsolaObjetos1.UI;

public sealed class AppState
{
    public const int RolCliente = 1;
    public const int RolEmpleado = 2;

    private readonly string connectionString;

    public AppState(string? connectionString = null)
    {
        this.connectionString = connectionString
            ?? Environment.GetEnvironmentVariable("WINFORMS_DB_CONNECTION")
            ?? "Server=localhost;Port=3306;Database=winforms_db;User ID=root;Password=;";
    }

    public WinformsDbContext CrearContexto()
    {
        DbContextOptions<WinformsDbContext> options = new DbContextOptionsBuilder<WinformsDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .Options;
        return new WinformsDbContext(options);
    }

    public List<Usuario> ObtenerUsuarios(int? rol = null, string? filtro = null)
    {
        using var context = CrearContexto();
        IQueryable<Usuario> query = context.Usuarios.AsNoTracking();
        if (rol.HasValue) query = query.Where(item => item.Rol == rol.Value);
        if (!string.IsNullOrWhiteSpace(filtro)) query = FiltrarUsuarios(query, filtro.Trim());
        return query.OrderBy(item => item.Nombre).ToList();
    }

    public Usuario AgregarUsuario(Usuario usuario)
    {
        using var context = CrearContexto();
        usuario.Id = 0;
        usuario.Codigo = SiguienteCodigoUsuario(context);
        context.Usuarios.Add(usuario);
        context.SaveChanges();
        return usuario;
    }

    public bool ActualizarUsuario(Usuario usuario)
    {
        using var context = CrearContexto();
        Usuario? existente = context.Usuarios.FirstOrDefault(item => item.Id == usuario.Id);
        if (existente is null) return false;
        existente.Codigo = usuario.Codigo;
        existente.Nombre = usuario.Nombre;
        existente.Correo = usuario.Correo;
        existente.Clave = usuario.Clave;
        existente.Activo = usuario.Activo;
        existente.Rol = usuario.Rol;
        existente.Direccion = usuario.Direccion;
        context.SaveChanges();
        return true;
    }

    public bool EliminarUsuario(int id)
    {
        using var context = CrearContexto();
        Usuario? usuario = context.Usuarios.FirstOrDefault(item => item.Id == id);
        if (usuario is null) return false;
        context.Usuarios.Remove(usuario);
        context.SaveChanges();
        return true;
    }

    public List<Producto> ObtenerProductos(string? filtro = null)
    {
        using var context = CrearContexto();
        IQueryable<Producto> query = context.Productos.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(filtro))
        {
            string criterio = filtro.Trim();
            query = query.Where(item => item.Codigo.ToString().Contains(criterio) || item.Nombre.Contains(criterio) || item.Categoria.Contains(criterio));
        }
        return query.OrderBy(item => item.Nombre).ToList();
    }

    public Producto AgregarProducto(Producto producto)
    {
        using var context = CrearContexto();
        producto.Codigo = SiguienteCodigoProducto(context);
        context.Productos.Add(producto);
        context.SaveChanges();
        return producto;
    }

    public bool ActualizarProducto(Producto producto)
    {
        using var context = CrearContexto();
        Producto? existente = context.Productos.FirstOrDefault(item => item.Codigo == producto.Codigo);
        if (existente is null) return false;
        existente.Nombre = producto.Nombre;
        existente.Categoria = producto.Categoria;
        existente.Descripcion = producto.Descripcion;
        existente.PrecioVenta = producto.PrecioVenta;
        existente.StockActual = producto.StockActual;
        existente.StockMinimo = producto.StockMinimo;
        existente.Activo = producto.Activo;
        existente.Impuesto = producto.Impuesto;
        context.SaveChanges();
        return true;
    }

    public bool EliminarProducto(int codigo)
    {
        using var context = CrearContexto();
        Producto? producto = context.Productos.FirstOrDefault(item => item.Codigo == codigo);
        if (producto is null) return false;
        context.Productos.Remove(producto);
        context.SaveChanges();
        return true;
    }

    public List<Venta> ObtenerVentas(string? filtro = null)
    {
        using var context = CrearContexto();
        IQueryable<Venta> query = context.Ventas.AsNoTracking().Include(item => item.Cliente).Include(item => item.Empleado).Include(item => item.Detallesventa).ThenInclude(item => item.ProductoCodigoNavigation);
        if (!string.IsNullOrWhiteSpace(filtro))
        {
            string criterio = filtro.Trim();
            query = query.Where(item => item.Codigo.ToString().Contains(criterio) || item.Cliente.Nombre.Contains(criterio) || item.Empleado.Nombre.Contains(criterio));
        }
        return query.OrderByDescending(item => item.FechaVenta).ToList();
    }

    public Venta CrearVenta(int clienteId, int empleadoId, IEnumerable<(int ProductoCodigo, int Cantidad)> detalles)
    {
        using var context = CrearContexto();
        Venta venta = new() { Codigo = SiguienteCodigoVenta(context), ClienteId = clienteId, EmpleadoId = empleadoId, FechaVenta = DateTime.Now };
        foreach ((int productoCodigo, int cantidad) in detalles)
        {
            Producto producto = context.Productos.First(item => item.Codigo == productoCodigo);
            venta.Detallesventa.Add(new Detallesventum { VentaCodigo = venta.Codigo, ProductoCodigo = producto.Codigo, Cantidad = cantidad, PrecioUnitario = producto.PrecioVenta, Impuesto = producto.Impuesto });
            producto.StockActual -= cantidad;
        }
        context.Ventas.Add(venta);
        context.SaveChanges();
        return venta;
    }

    public bool EliminarVenta(int codigo)
    {
        using var context = CrearContexto();
        Venta? venta = context.Ventas.FirstOrDefault(item => item.Codigo == codigo);
        if (venta is null) return false;
        context.Ventas.Remove(venta);
        context.SaveChanges();
        return true;
    }

    public int SiguienteCodigoUsuario(int rol)
    {
        using var context = CrearContexto();
        return (context.Usuarios.Where(item => item.Rol == rol).Select(item => (int?)item.Codigo).Max() ?? 0) + 1;
    }

    public int SiguienteCodigoProducto()
    {
        using var context = CrearContexto();
        return SiguienteCodigoProducto(context);
    }

    public int SiguienteCodigoVenta()
    {
        using var context = CrearContexto();
        return SiguienteCodigoVenta(context);
    }

    private static IQueryable<Usuario> FiltrarUsuarios(IQueryable<Usuario> query, string criterio) => query.Where(item => item.Codigo.ToString().Contains(criterio) || item.Nombre.Contains(criterio) || item.Correo.Contains(criterio));
    private static int SiguienteCodigoUsuario(WinformsDbContext context) => (context.Usuarios.Select(item => (int?)item.Codigo).Max() ?? 0) + 1;
    private static int SiguienteCodigoProducto(WinformsDbContext context) => (context.Productos.Select(item => (int?)item.Codigo).Max() ?? 0) + 1;
    private static int SiguienteCodigoVenta(WinformsDbContext context) => (context.Ventas.Select(item => (int?)item.Codigo).Max() ?? 0) + 1;
}
