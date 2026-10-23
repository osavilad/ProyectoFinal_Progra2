# ProyectoFinal_Progra2
# Sistema de Control Clínico Veterinario

Aplicación de consola (C# / .NET 8) para administrar propietarios, mascotas, veterinarios y consultas
(historial clínico). Usa ADO.NET con SQLite como base de datos local, con la misma configuración del
proyecto Sistema_Prestamo_Biblioteca.

## Tecnologías
| Tecnología | Uso |
| --- | --- |
| C# / .NET 8 | Lenguaje y plataforma (`net8.0`). |
| ADO.NET | Conexiones, comandos y parámetros. |
| SQLite | Base de datos local en un archivo; no requiere servidor. |
| `Microsoft.Data.Sqlite` 8.0.11 | Proveedor ADO.NET. |

## Ejecutar
```powershell
dotnet restore
dotnet build
dotnet run
```

## Base de datos
Por defecto el archivo se crea en:
```text
%LOCALAPPDATA%\UMG\ClinicaVeterinaria\clinica.db
```
Para usar otra ubicación, defina la variable de entorno `CLINICA_VETERINARIA_DB`:
```powershell
$env:CLINICA_VETERINARIA_DB = "C:\datos\clinica.db"
dotnet run
```
El esquema está en `crear_tablas.sql` (recurso embebido); `InicializadorBaseDatos` lo ejecuta al iniciar.

## Estructura
```text
ClinicaVeterinaria/
├── Dominio/        Propietario, Mascota, Veterinario, Consulta
├── Persistencia/   InicializadorBaseDatos, AccesoDatos, Repositorio* (CRUD)
├── Aplicacion/     MenuClinica, Consola
├── crear_tablas.sql
├── Program.cs
└── ClinicaVeterinaria.csproj
```
