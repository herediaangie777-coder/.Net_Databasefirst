using System;
using System.Collections.Generic;

namespace ProyectoConsolaObjetos1.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public int Codigo { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Clave { get; set; } = null!;

    public bool Activo { get; set; }

    public int Rol { get; set; }

    public string? Direccion { get; set; }

    public virtual ICollection<Venta> VentaClientes { get; set; } = new List<Venta>();

    public virtual ICollection<Venta> VentaEmpleados { get; set; } = new List<Venta>();
}
