using System;
using ProyectoConsolaObjetos1.Models.Base;

namespace ProyectoConsolaObjetos1.Models.Users
{
    public class Cliente : Usuario
    {
        private string direccion = string.Empty;

        public string Direccion
        {
            get { return this.direccion; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("La direccion no puede estar vacia o ser nula.");
                }
                this.direccion = value;
            }
        }

        public bool ValidarSetCampos()
        {
            if (string.IsNullOrEmpty(this.Nombre) || string.IsNullOrEmpty(this.Correo) || string.IsNullOrEmpty(this.direccion))
            {
                return false;
            }
            return true;
        }

        public void MostrarPanelCliente()
        {
            Console.WriteLine("===== PANEL DEL CLIENTE =====");
            Console.WriteLine("Codigo: " + this.Codigo);
            Console.WriteLine("Nombre: " + this.Nombre);
            Console.WriteLine("Correo: " + this.Correo);
            Console.WriteLine("Direccion: " + this.direccion);
            Console.WriteLine("Activo: " + (this.Activo ? "Si" : "No"));
            Console.WriteLine("==============================");
        }
    }
}

