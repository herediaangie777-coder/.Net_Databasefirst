namespace ProyectoConsolaObjetos1.Models.Details;

public sealed class DetalleVenta
{
    public int ProductoCodigo { get; set; }
    public string ProductoNombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public float PrecioUnitario { get; set; }
    public float Subtotal => Cantidad * PrecioUnitario;
}