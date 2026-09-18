using System.ComponentModel;
using ProyectoConsolaObjetos1.Models.Academic;
using ProyectoConsolaObjetos1.Models.Base;
using ProyectoConsolaObjetos1.Models.Details;
using ProyectoConsolaObjetos1.Models.Users;
using ProyectoConsolaObjetos1.Services;

namespace ProyectoConsolaObjetos1.UI;

public sealed class AppState
{
    public BindingList<Usuario> Usuarios { get; } = new();
    public BindingList<Cliente> Clientes { get; } = new();
    public BindingList<Empleado> Empleados { get; } = new();
    public BindingList<Producto> Productos { get; } = new();
    public BindingList<Venta> Ventas { get; } = new();
    public BindingList<DetalleVenta> DetallesVenta { get; } = new();

    public AppState()
    {
        CargarDatos();
    }

    public void CargarDatos()
    {
        CargarOAgregarDemo(Usuarios, "usuarios.json", CrearUsuariosDemo);
        CargarOAgregarDemo(Clientes, "clientes.json", CrearClientesDemo);
        CargarOAgregarDemo(Empleados, "empleados.json", CrearEmpleadosDemo);
        CargarOAgregarDemo(Productos, "productos.json", CrearProductosDemo);
        CargarOAgregarDemo(Ventas, "ventas.json", CrearVentasDemo);
        ActualizarDetalles();
    }

    public void GuardarTodo()
    {
        new JsonRepository<Usuario>("usuarios.json").Save(Usuarios);
        new JsonRepository<Cliente>("clientes.json").Save(Clientes);
        new JsonRepository<Empleado>("empleados.json").Save(Empleados);
        new JsonRepository<Producto>("productos.json").Save(Productos);
        new JsonRepository<Venta>("ventas.json").Save(Ventas);
    }

    public void CargarOAgregarDemo<T>(BindingList<T> lista, string archivo, Func<List<T>> crearDemo)
    {
        List<T> datos = new JsonRepository<T>(archivo).Load();
        lista.Clear();
        IEnumerable<T> elementos = datos.Count > 0 ? datos : crearDemo();
        foreach (T elemento in elementos)
        {
            lista.Add(elemento);
        }
    }

    public void ActualizarDetalles()
    {
        DetallesVenta.Clear();
        foreach (Venta venta in Ventas)
        {
            foreach (DetalleVenta detalle in venta.Detalles)
            {
                DetallesVenta.Add(detalle);
            }
        }
    }

    private static List<Usuario> CrearUsuariosDemo() => new()
    {
        CrearUsuario(1, "Ana Torres", "ana@tienda.com"),
        CrearUsuario(2, "Luis Mendoza", "luis@tienda.com"),
        CrearUsuario(3, "Sofia Rojas", "sofia@tienda.com")
    };

    private static List<Cliente> CrearClientesDemo() => new()
    {
        new Cliente { Codigo = 1, Nombre = "Maria Lopez", Correo = "maria@email.com", Direccion = "Av. Principal 123", Activo = true },
        new Cliente { Codigo = 2, Nombre = "Juan Garcia", Correo = "juan@email.com", Direccion = "Calle Secundaria 456", Activo = true },
        new Cliente { Codigo = 3, Nombre = "Elena Castro", Correo = "elena@email.com", Direccion = "Jiron Lima 789", Activo = true }
    };

    private static List<Empleado> CrearEmpleadosDemo() => new()
    {
        CrearEmpleado(1, "Carlos Perez", "carlos@tienda.com"),
        CrearEmpleado(2, "Rosa Diaz", "rosa@tienda.com"),
        CrearEmpleado(3, "Diego Silva", "diego@tienda.com")
    };

    private static List<Producto> CrearProductosDemo() => new()
    {
        new Producto { Codigo = 1, Nombre = "Laptop HP", Categoria = "Electronica", Descripcion = "Laptop de 15 pulgadas", PrecioVenta = 2500f, StockActual = 10, StockMinimo = 2, Impuesto = 0.19f, Activo = true },
        new Producto { Codigo = 2, Nombre = "Mouse inalambrico", Categoria = "Accesorios", Descripcion = "Mouse ergonomico", PrecioVenta = 45.50f, StockActual = 25, StockMinimo = 5, Impuesto = 0.19f, Activo = true },
        new Producto { Codigo = 3, Nombre = "Teclado mecanico", Categoria = "Accesorios", Descripcion = "Teclado con iluminacion RGB", PrecioVenta = 120f, StockActual = 15, StockMinimo = 3, Impuesto = 0.19f, Activo = true }
    };

    private static List<Venta> CrearVentasDemo()
    {
        List<Cliente> clientes = CrearClientesDemo();
        List<Empleado> empleados = CrearEmpleadosDemo();
        List<Producto> productos = CrearProductosDemo();
        List<Venta> ventas = new();
        for (int indice = 0; indice < 3; indice++)
        {
            Venta venta = new()
            {
                Codigo = indice + 1,
                Cliente = clientes[indice],
                Empleado = empleados[indice],
                FechaVenta = DateTime.Today.AddDays(-indice)
            };
            venta.AgregarProducto(productos[indice]);
            ventas.Add(venta);
        }
        return ventas;
    }

    private static Usuario CrearUsuario(int codigo, string nombre, string correo) => new()
    {
        Codigo = codigo, Nombre = nombre, Correo = correo, Clave = "demo", Activo = true
    };

    private static Empleado CrearEmpleado(int codigo, string nombre, string correo) => new()
    {
        Codigo = codigo, Nombre = nombre, Correo = correo, Clave = "demo", Activo = true
    };
}
