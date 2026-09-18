using System;

namespace ProyectoConsolaObjetos1.Models.Academic
{
    public class Producto
    {
        private int codigo;
        private string nombre = string.Empty;
        private string categoria = string.Empty;
        private string descripcion = string.Empty;
        private float precioVenta;
        private int stockActual;
        private int stockMinimo;
        private bool activo;
        private float impuesto = 0.19f;

        public int Codigo
        {
            get { return this.codigo; }
            set { this.codigo = value; }
        }

        public string Nombre
        {
            get { return this.nombre; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("El nombre del producto no puede estar vacio o ser nulo.");
                }
                this.nombre = value;
            }
        }

        public string Categoria
        {
            get { return this.categoria; }
            set { this.categoria = value; }
        }

        public string Descripcion
        {
            get { return this.descripcion; }
            set { this.descripcion = value; }
        }

        public float PrecioVenta
        {
            get { return this.precioVenta; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("El precio de venta no puede ser negativo.");
                }
                this.precioVenta = value;
            }
        }

        public int StockActual
        {
            get { return this.stockActual; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("El stock actual no puede ser negativo.");
                }
                this.stockActual = value;
            }
        }

        public int StockMinimo
        {
            get { return this.stockMinimo; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("El stock minimo no puede ser negativo.");
                }
                this.stockMinimo = value;
            }
        }

        public bool Activo
        {
            get { return this.activo; }
            set { this.activo = value; }
        }

        public float Impuesto
        {
            get { return this.impuesto; }
            set { this.impuesto = value; }
        }

        public bool ValidarSetCampos()
        {
            if (string.IsNullOrEmpty(this.nombre) || string.IsNullOrEmpty(this.categoria) ||
                this.precioVenta <= 0 || this.stockActual < 0 || this.stockMinimo < 0)
            {
                return false;
            }
            return true;
        }
    }
}

