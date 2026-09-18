# ProyectoConsolaObjetos1

Aplicacion de Windows Forms sobre .NET 8 para administrar usuarios, productos y ventas mediante Entity Framework Core con MySQL Database First.

## Requisitos

- .NET 8 SDK.
- MySQL Server con la base de datos `winforms_db`.
- Herramienta `dotnet-ef`:

```powershell
dotnet tool install --global dotnet-ef
```

## Configuracion

1. Copia `.env.example` como `.env`.
2. Ajusta las credenciales de MySQL en `.env`.
3. Restaura y compila el proyecto:

```powershell
dotnet restore
dotnet build
```

## Scaffolding Database First

El modelo se genero con el siguiente comando, especificando las cuatro tablas y separando los namespaces del contexto y de las entidades:

```powershell
dotnet ef dbcontext scaffold "server=localhost;port=3306;database=winforms_db;user=root;password=;" Pomelo.EntityFrameworkCore.MySql --context WinformsDbContext --context-dir Data --output-dir Models --namespace ProyectoConsolaObjetos1.Models --context-namespace ProyectoConsolaObjetos1.Data -t detallesventa -t productos -t usuarios -t ventas --no-onconfiguring --force
```

`--no-onconfiguring` mantiene la cadena de conexion fuera del archivo generado. `--force` permite regenerar el modelo cuando cambia la base de datos; la configuracion manual vive en `Data/WinformsDbContext.Partial.cs` y no se pierde.

## Analisis de Perdida y Conservacion de Dominio

Las tablas se mapearon como entidades persistentes: `Usuario`, `Producto`, `Venta` y `Detallesventum`. `Venta` agrega una coleccion de detalles, mientras que cada detalle referencia un producto y su venta. Esta estructura representa la agregacion de una venta con sus lineas y conserva las relaciones mediante claves foraneas y propiedades de navegacion.

La tabla `usuarios` almacena clientes y empleados en una sola entidad. La aplicacion simula una estrategia TPH (Table Per Hierarchy) usando la columna `Rol` como discriminador logico: el valor `1` representa cliente y `2` representa empleado. `UserTypes` conserva los nombres de dominio `Cliente` y `Empleado`; la persistencia actual mantiene el valor entero existente en la base de datos.

Durante el scaffolding se recuperan tablas, columnas, claves, indices, restricciones y relaciones, pero no se recuperan comportamientos propios de C# que no existen en el esquema: clases derivadas, nombres semanticos para roles, validaciones, reglas de negocio, servicios, constructores personalizados ni configuracion de conexion segura. Ademas, el scaffolding puede sobrescribir archivos generados cuando se usa `--force`.

La conservacion se logra manteniendo `WinformsDbContext.cs` como salida limpia del scaffolding y colocando la configuracion en `WinformsDbContext.Partial.cs`. Las clases parciales y `UserTypes` permiten agregar comportamiento y vocabulario de dominio sin modificar el codigo generado. `AppState` recupera las reglas de negocio de CRUD, carga explicita de relaciones y actualizacion segura de entidades rastreadas.

## Ejecucion

```powershell
dotnet run
```

La aplicacion requiere que MySQL este disponible y que `.env` contenga una configuracion valida.
