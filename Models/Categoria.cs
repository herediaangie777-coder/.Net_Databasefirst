using System;
using System.Collections.Generic;

namespace ProyectoConsolaObjetos1.Models;

public partial class Categoria
{
    public int IdCategoria { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }
}
