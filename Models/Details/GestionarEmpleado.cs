using ProyectoConsolaObjetos1.Models.Users;
using ProyectoConsolaObjetos1.Services;

namespace ProyectoConsolaObjetos1.Models.Details;

public sealed class GestionarEmpleado
{
    private readonly JsonRepository<Empleado> repository;

    public List<Empleado> ListadoEmpleados { get; private set; }

    public GestionarEmpleado(JsonRepository<Empleado>? repository = null)
    {
        this.repository = repository ?? new JsonRepository<Empleado>("empleados.json");
        ListadoEmpleados = new List<Empleado>();
    }

    public List<Empleado> CargarEmpleados()
    {
        ListadoEmpleados = repository.Load();
        return new List<Empleado>(ListadoEmpleados);
    }

    public bool CrearEmpleado(Empleado empleado)
    {
        if (empleado == null || string.IsNullOrWhiteSpace(empleado.Nombre) ||
            string.IsNullOrWhiteSpace(empleado.Correo))
        {
            return false;
        }

        if (empleado.Codigo <= 0)
        {
            empleado.Codigo = repository.GetNextId(ListadoEmpleados, item => item.Codigo);
        }

        if (ListadoEmpleados.Any(item => item.Codigo == empleado.Codigo))
        {
            return false;
        }

        empleado.Activo = true;
        ListadoEmpleados.Add(empleado);
        repository.Save(ListadoEmpleados);
        return true;
    }

    public void ActualizarDatos()
    {
        CargarEmpleados();
    }
}