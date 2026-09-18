using System;
using System.Collections.Generic;

namespace ProyectoConsolaObjetos1.Models;

public partial class Producto
{
    public int Codigo { get; set; }

    public string Nombre { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public decimal PrecioVenta { get; set; }

    public int StockActual { get; set; }

    public int StockMinimo { get; set; }

    public bool Activo { get; set; }

    public float Impuesto { get; set; }

    public virtual ICollection<Detallesventum> Detallesventa { get; set; } = new List<Detallesventum>();
}
