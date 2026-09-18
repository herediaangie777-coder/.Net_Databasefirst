using System;
using System.Security.Cryptography;
using System.Text;

namespace ProyectoConsolaObjetos1.Models.Base
{
    public class Usuario
    {
        private int codigo;
        private string nombre = string.Empty;
        private string correo = string.Empty;
        private string clave = string.Empty;
        private bool activo;

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
                    throw new ArgumentException("El nombre no puede estar vacio o ser nulo.");
                }
                this.nombre = value;
            }
        }

        public string Correo
        {
            get { return this.correo; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("El correo no puede estar vacio o ser nulo.");
                }
                this.correo = value;
            }
        }

        public string Clave
        {
            get { return this.clave; }
            set { this.clave = value; }
        }

        public bool Activo
        {
            get { return this.activo; }
            set { this.activo = value; }
        }

        public bool ValidarAcceso(string correo, string clave)
        {
            if (this.correo == correo && this.clave == clave && this.activo)
            {
                return true;
            }
            return false;
        }

        public void InactivarUsuario()
        {
            this.activo = false;
        }

        public string CifarClave(string claveOriginal)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(claveOriginal));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}

