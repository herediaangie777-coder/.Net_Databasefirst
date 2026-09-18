using System;
using System.Collections.Generic;
using ProyectoConsolaObjetos1.Models.Details;
using ProyectoConsolaObjetos1.Services;

namespace ProyectoConsolaObjetos1.Models.Academic
{
    public class GestionarVenta
    {
        private List<Venta> historialVentas;
        private readonly JsonRepository<Venta> repository;

        public List<Venta> HistorialVentas
        {
            get { return this.historialVentas; }
            set { this.historialVentas = value; }
        }

        public GestionarVenta(JsonRepository<Venta>? repository = null)
        {
            this.historialVentas = new List<Venta>();
            this.repository = repository ?? new JsonRepository<Venta>("ventas.json");
        }

        public List<Venta> CargarVentas()
        {
            this.historialVentas = repository.Load();
            return new List<Venta>(this.historialVentas);
        }

        public bool CrearVenta(Venta v, GestionarProducto gestorProducto)
        {
            if (v == null || v.Cliente == null || v.Productos == null || v.Productos.Count == 0)
            {
                return false;
            }

            foreach (Producto p in v.Productos)
            {
                if (p.StockActual <= 0)
                {
                    return false;
                }
            }

            foreach (Producto p in v.Productos)
            {
                gestorProducto.ActualizarStock(p.Codigo, p.StockActual - 1);
            }

            this.historialVentas.Add(v);
            repository.Save(this.historialVentas);
            return true;
        }

        public void ActualizarDatos()
        {
            CargarVentas();
        }

        public void GenerarComprobante(Venta v)
        {
            Console.WriteLine("============================================");
            Console.WriteLine("            COMPROBANTE DE VENTA            ");
            Console.WriteLine("============================================");
            Console.WriteLine("Codigo Venta: " + v.Codigo);
            Console.WriteLine("Fecha: " + v.FechaVenta.ToString("dd/MM/yyyy HH:mm:ss"));
            Console.WriteLine("Cliente: " + v.Cliente.Nombre + " (" + v.Cliente.Correo + ")");
            Console.WriteLine("Empleado: " + v.Empleado.Nombre);
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("Productos:");
            foreach (Producto p in v.Productos)
            {
                Console.WriteLine("  - " + p.Nombre + " (Cod: " + p.Codigo + ")  Precio: S/ " + p.PrecioVenta.ToString("F2"));
            }
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("Subtotal: S/ " + v.CalcularSubtotal().ToString("F2"));
            float impuestoTotal = 0f;
            foreach (Producto p in v.Productos)
            {
                impuestoTotal += p.PrecioVenta * p.Impuesto;
            }
            Console.WriteLine("Impuesto (19%): S/ " + impuestoTotal.ToString("F2"));
            Console.WriteLine("Total: S/ " + v.CalcularTotal().ToString("F2"));
            Console.WriteLine("============================================");
        }

        public List<Venta> BuscarVentas(int codigoVenta)
        {
            List<Venta> resultados = new List<Venta>();
            foreach (Venta v in this.historialVentas)
            {
                if (v.Codigo == codigoVenta)
                {
                    resultados.Add(v);
                }
            }
            return resultados;
        }

        public void GenerarReporteVentasFecha(DateTime inicio, DateTime fin)
        {
            Console.WriteLine("============================================");
            Console.WriteLine("    REPORTE DE VENTAS POR RANGO DE FECHAS  ");
            Console.WriteLine("Desde: " + inicio.ToString("dd/MM/yyyy"));
            Console.WriteLine("Hasta: " + fin.ToString("dd/MM/yyyy"));
            Console.WriteLine("============================================");

            int contador = 0;
            float totalGeneral = 0f;

            foreach (Venta v in this.historialVentas)
            {
                if (v.FechaVenta >= inicio && v.FechaVenta <= fin)
                {
                    contador++;
                    totalGeneral += v.CalcularTotal();
                    Console.WriteLine("Venta #" + contador + " - Codigo: " + v.Codigo +
                                      " | Cliente: " + v.Cliente.Nombre +
                                      " | Total: S/ " + v.CalcularTotal().ToString("F2") +
                                      " | Fecha: " + v.FechaVenta.ToString("dd/MM/yyyy"));
                }
            }

            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("Total de Ventas: " + contador);
            Console.WriteLine("Monto Total General: S/ " + totalGeneral.ToString("F2"));
            Console.WriteLine("============================================");
        }
    }
}

