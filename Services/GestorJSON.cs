using System.Collections.Generic;

namespace ProyectoConsolaObjetos1.Services
{
    /// <summary>Compatibilidad con el nombre usado por la aplicación existente.</summary>
    public static class GestorJSON
    {
        public static void GuardarJson<T>(string nombreArchivo, List<T> datos)
        {
            new JsonRepository<T>(nombreArchivo).Save(datos);
        }

        public static List<T> CargarJson<T>(string nombreArchivo)
        {
            return new JsonRepository<T>(nombreArchivo).Load();
        }
    }
}
