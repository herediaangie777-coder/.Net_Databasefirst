using System;
using ProyectoConsolaObjetos1.Models.Base;

namespace ProyectoConsolaObjetos1.Models.Users
{
    public class Empleado : Usuario
    {
        public void MostrarPanelEmpleado()
        {
            Console.WriteLine("===== PANEL DEL EMPLEADO =====");
            Console.WriteLine("Codigo: " + this.Codigo);
            Console.WriteLine("Nombre: " + this.Nombre);
            Console.WriteLine("Correo: " + this.Correo);
            Console.WriteLine("Activo: " + (this.Activo ? "Si" : "No"));
            Console.WriteLine("================================");
        }
    }
}

