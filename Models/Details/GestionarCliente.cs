using System;
using System.Collections.Generic;
using ProyectoConsolaObjetos1.Models.Users;
using ProyectoConsolaObjetos1.Services;

namespace ProyectoConsolaObjetos1.Models.Details
{
    public class GestionarCliente
    {
        private List<Cliente> listadoClientes;
        private readonly JsonRepository<Cliente> repository;

        public List<Cliente> ListadoClientes
        {
            get { return this.listadoClientes; }
            set { this.listadoClientes = value; }
        }

        public GestionarCliente(JsonRepository<Cliente>? repository = null)
        {
            this.listadoClientes = new List<Cliente>();
            this.repository = repository ?? new JsonRepository<Cliente>("clientes.json");
        }

        public bool CrearCliente(Cliente c)
        {
            if (c == null || !c.ValidarSetCampos())
            {
                return false;
            }

            if (c.Codigo <= 0)
            {
                c.Codigo = repository.GetNextId(this.listadoClientes, item => item.Codigo);
            }

            foreach (Cliente existente in this.listadoClientes)
            {
                if (existente.Codigo == c.Codigo)
                {
                    return false;
                }
            }

            c.Correo = c.Correo;
            c.Activo = true;
            this.listadoClientes.Add(c);
            repository.Save(this.listadoClientes);
            return true;
        }

        public List<Cliente> BuscarClientes(string criterio)
        {
            List<Cliente> resultados = new List<Cliente>();
            foreach (Cliente c in this.listadoClientes)
            {
                if (c.Nombre.ToLower().Contains(criterio.ToLower()) ||
                    c.Correo.ToLower().Contains(criterio.ToLower()) ||
                    c.Direccion.ToLower().Contains(criterio.ToLower()))
                {
                    resultados.Add(c);
                }
            }
            return resultados;
        }

        public List<Cliente> ListarClientes()
        {
            return new List<Cliente>(this.listadoClientes);
        }

        public bool ActualizarClientes(int codigo, string nuevaDireccion, string nuevoCorreo)
        {
            foreach (Cliente c in this.listadoClientes)
            {
                if (c.Codigo == codigo)
                {
                    if (!string.IsNullOrEmpty(nuevaDireccion))
                    {
                        c.Direccion = nuevaDireccion;
                    }
                    if (!string.IsNullOrEmpty(nuevoCorreo))
                    {
                        c.Correo = nuevoCorreo;
                    }
                    return true;
                }
            }
            return false;
        }

        public bool DesactivarCliente(int codigo)
        {
            foreach (Cliente c in this.listadoClientes)
            {
                if (c.Codigo == codigo)
                {
                    c.InactivarUsuario();
                    return true;
                }
            }
            return false;
        }

        public void GenrerarReporteClientes()
        {
            Console.WriteLine("============================================");
            Console.WriteLine("         REPORTE DE CLIENTES                ");
            Console.WriteLine("============================================");
            Console.WriteLine("Total de Clientes Registrados: " + this.listadoClientes.Count);
            Console.WriteLine("--------------------------------------------");

            foreach (Cliente c in this.listadoClientes)
            {
                Console.WriteLine("Codigo: " + c.Codigo);
                Console.WriteLine("Nombre: " + c.Nombre);
                Console.WriteLine("Correo: " + c.Correo);
                Console.WriteLine("Direccion: " + c.Direccion);
                Console.WriteLine("Activo: " + (c.Activo ? "Si" : "No"));
                Console.WriteLine("--------------------------------------------");
            }
            Console.WriteLine("============================================");
        }
    }
}

