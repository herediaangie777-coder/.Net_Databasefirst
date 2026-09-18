using System;
using System.Collections.Generic;
using ProyectoConsolaObjetos1.Models.Details;
using ProyectoConsolaObjetos1.Models.Users;

namespace ProyectoConsolaObjetos1.Models.Academic
{
    public class Venta
    {
        private int codigo;
        private Cliente cliente = null!;
        private List<Producto> productos;
        private List<DetalleVenta> detalles;
        private DateTime fechaVenta;
        private Empleado empleado = null!;

        public int Codigo
        {
            get { return this.codigo; }
            set { this.codigo = value; }
        }

        public Cliente Cliente
        {
            get { return this.cliente; }
            set { this.cliente = value; }
        }

        public List<Producto> Productos
        {
            get { return this.productos; }
            set { this.productos = value; }
        }

        public List<DetalleVenta> Detalles
        {
            get { return this.detalles; }
            set { this.detalles = value ?? new List<DetalleVenta>(); }
        }

        public DateTime FechaVenta
        {
            get { return this.fechaVenta; }
            set { this.fechaVenta = value; }
        }

        public Empleado Empleado
        {
            get { return this.empleado; }
            set { this.empleado = value; }
        }

        public Venta()
        {
            this.productos = new List<Producto>();
            this.detalles = new List<DetalleVenta>();
        }

        public void AgregarProducto(Producto p)
        {
            if (p != null && p.StockActual > 0)
            {
                this.productos.Add(p);
                this.detalles.Add(new DetalleVenta
                {
                    ProductoCodigo = p.Codigo,
                    ProductoNombre = p.Nombre,
                    Cantidad = 1,
                    PrecioUnitario = p.PrecioVenta
                });
            }
        }

        public float CalcularSubtotal()
        {
            float subtotal = 0f;
            foreach (Producto p in this.productos)
            {
                subtotal += p.PrecioVenta;
            }
            return subtotal;
        }

        public float CalcularTotal()
        {
            float subtotal = this.CalcularSubtotal();
            float impuestoTotal = 0f;
            foreach (Producto p in this.productos)
            {
                impuestoTotal += p.PrecioVenta * p.Impuesto;
            }
            return subtotal + impuestoTotal;
        }
    }
}

