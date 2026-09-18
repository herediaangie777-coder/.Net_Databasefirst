using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace ProyectoConsolaObjetos1.Data;

public partial class WinformsDbContext
{
    public WinformsDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        CargarArchivoEnv();

        MySqlConnectionStringBuilder connection = new()
        {
            Server = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost",
            Port = ObtenerPuerto(),
            Database = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "winforms_db",
            UserID = Environment.GetEnvironmentVariable("DB_USER") ?? "root",
            Password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? string.Empty
        };

        optionsBuilder.UseMySql(connection.ConnectionString, ServerVersion.AutoDetect(connection.ConnectionString));
    }

    private static void CargarArchivoEnv()
    {
        string envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
        if (File.Exists(envPath))
        {
            Env.Load(envPath);
        }
    }

    private static uint ObtenerPuerto()
    {
        return uint.TryParse(Environment.GetEnvironmentVariable("DB_PORT"), out uint port)
            ? port
            : 3306;
    }
}
