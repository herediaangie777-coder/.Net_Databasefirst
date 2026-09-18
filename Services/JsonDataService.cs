using ProyectoConsolaObjetos1.Models.Academic;
using ProyectoConsolaObjetos1.Models.Users;

namespace ProyectoConsolaObjetos1.Services;

public sealed class JsonDataService
{
    public JsonRepository<Cliente> Clientes { get; } = new("clientes.json");
    public JsonRepository<Empleado> Empleados { get; } = new("empleados.json");
    public JsonRepository<Producto> Productos { get; } = new("productos.json");
    public JsonRepository<Venta> Ventas { get; } = new("ventas.json");
}