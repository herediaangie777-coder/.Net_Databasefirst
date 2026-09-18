using System;
using System.Collections.Generic;
using ProyectoConsolaObjetos1.Models.Academic;
using ProyectoConsolaObjetos1.Services;

namespace ProyectoConsolaObjetos1.Models.Details
{
    public class GestionarProducto
    {
        private List<Producto> listadoProductos;
        private readonly JsonRepository<Producto> repository;

        public List<Producto> ListadoProductos
        {
            get { return this.listadoProductos; }
            set { this.listadoProductos = value; }
        }

        public GestionarProducto(JsonRepository<Producto>? repository = null)
        {
            this.listadoProductos = new List<Producto>();
            this.repository = repository ?? new JsonRepository<Producto>("productos.json");
        }

        public bool CrearProducto(Producto p)
        {
            if (p == null || !p.ValidarSetCampos())
            {
                return false;
            }

            if (p.Codigo <= 0)
            {
                p.Codigo = repository.GetNextId(this.listadoProductos, item => item.Codigo);
            }

            foreach (Producto existente in this.listadoProductos)
            {
                if (existente.Codigo == p.Codigo)
                {
                    return false;
                }
            }

            p.Activo = true;
            this.listadoProductos.Add(p);
            repository.Save(this.listadoProductos);
            return true;
        }

        public bool ActualizarProducto(int codigo, float nuevoPrecio, int nuevoStockMin)
        {
            foreach (Producto p in this.listadoProductos)
            {
                if (p.Codigo == codigo)
                {
                    if (nuevoPrecio >= 0)
                    {
                        p.PrecioVenta = nuevoPrecio;
                    }
                    if (nuevoStockMin >= 0)
                    {
                        p.StockMinimo = nuevoStockMin;
                    }
                    return true;
                }
            }
            return false;
        }

        public List<Producto> ListarProductos()
        {
            return new List<Producto>(this.listadoProductos);
        }

        public List<Producto> BuscarProductos(string criterio)
        {
            List<Producto> resultados = new List<Producto>();
            foreach (Producto p in this.listadoProductos)
            {
                if (p.Nombre.ToLower().Contains(criterio.ToLower()) ||
                    p.Categoria.ToLower().Contains(criterio.ToLower()) ||
                    p.Descripcion.ToLower().Contains(criterio.ToLower()))
                {
                    resultados.Add(p);
                }
            }
            return resultados;
        }

        public bool DesactivarProducto(int codigo, List<Venta> historialVentas)
        {
            if (ValidarVentaAsociada(codigo, historialVentas))
            {
                Console.WriteLine("No se puede desactivar el producto porque tiene ventas asociadas.");
                return false;
            }

            foreach (Producto p in this.listadoProductos)
            {
                if (p.Codigo == codigo)
                {
                    p.Activo = false;
                    return true;
                }
            }
            return false;
        }

        public bool ValidarVentaAsociada(int codigoProducto, List<Venta> historialVentas)
        {
            if (historialVentas == null)
            {
                return false;
            }

            foreach (Venta v in historialVentas)
            {
                foreach (Producto p in v.Productos)
                {
                    if (p.Codigo == codigoProducto)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ValidarStock()
        {
            Console.WriteLine("============================================");
            Console.WriteLine("        VALIDACION DE STOCK                 ");
            Console.WriteLine("============================================");

            foreach (Producto p in this.listadoProductos)
            {
                if (p.StockActual <= p.StockMinimo)
                {
                    Console.WriteLine("ALERTA: Producto '" + p.Nombre + "' (Cod: " + p.Codigo +
                                      ") tiene stock bajo. Actual: " + p.StockActual +
                                      ", Minimo: " + p.StockMinimo);
                }
                else
                {
                    Console.WriteLine("OK: Producto '" + p.Nombre + "' (Cod: " + p.Codigo +
                                      ") Stock actual: " + p.StockActual);
                }
            }
            Console.WriteLine("============================================");
        }

        public void ActualizarStock(int codigoProducto, int cantidad)
        {
            foreach (Producto p in this.listadoProductos)
            {
                if (p.Codigo == codigoProducto)
                {
                    p.StockActual = cantidad;
                    return;
                }
            }
        }

        public void GenerarReporteProductosMasVendidos(List<Venta> ventas)
        {
            Console.WriteLine("============================================");
            Console.WriteLine("    REPORTE DE PRODUCTOS MAS VENDIDOS      ");
            Console.WriteLine("============================================");

            Dictionary<int, int> contadorProductos = new Dictionary<int, int>();

            if (ventas != null)
            {
                foreach (Venta v in ventas)
                {
                    foreach (Producto p in v.Productos)
                    {
                        if (contadorProductos.ContainsKey(p.Codigo))
                        {
                            contadorProductos[p.Codigo]++;
                        }
                        else
                        {
                            contadorProductos[p.Codigo] = 1;
                        }
                    }
                }
            }

            if (contadorProductos.Count == 0)
            {
                Console.WriteLine("No hay ventas registradas para generar el reporte.");
            }
            else
            {
                foreach (KeyValuePair<int, int> entry in contadorProductos)
                {
                    foreach (Producto p in this.listadoProductos)
                    {
                        if (p.Codigo == entry.Key)
                        {
                            Console.WriteLine("Producto: " + p.Nombre + " (Cod: " + p.Codigo +
                                              ") | Veces vendido: " + entry.Value);
                            break;
                        }
                    }
                }
            }
            Console.WriteLine("============================================");
        }

        public void GenerarReporteInventario()
        {
            Console.WriteLine("============================================");
            Console.WriteLine("         REPORTE DE INVENTARIO              ");
            Console.WriteLine("============================================");
            Console.WriteLine("Total de Productos Registrados: " + this.listadoProductos.Count);
            Console.WriteLine("--------------------------------------------");

            foreach (Producto p in this.listadoProductos)
            {
                Console.WriteLine("Codigo: " + p.Codigo);
                Console.WriteLine("Nombre: " + p.Nombre);
                Console.WriteLine("Categoria: " + p.Categoria);
                Console.WriteLine("Descripcion: " + p.Descripcion);
                Console.WriteLine("Precio Venta: S/ " + p.PrecioVenta.ToString("F2"));
                Console.WriteLine("Stock Actual: " + p.StockActual);
                Console.WriteLine("Stock Minimo: " + p.StockMinimo);
                Console.WriteLine("Activo: " + (p.Activo ? "Si" : "No"));
                Console.WriteLine("Impuesto: " + (p.Impuesto * 100) + "%");
                Console.WriteLine("--------------------------------------------");
            }
            Console.WriteLine("============================================");
        }
    }
}

