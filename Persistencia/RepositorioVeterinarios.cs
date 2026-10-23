using ClinicaVeterinaria.Dominio;
using Microsoft.Data.Sqlite;
using static ClinicaVeterinaria.Persistencia.AccesoDatos;

namespace ClinicaVeterinaria.Persistencia;

internal sealed class RepositorioVeterinarios
{
    private readonly AccesoDatos _db;
    public RepositorioVeterinarios(AccesoDatos db) => _db = db;

    private static Veterinario Mapear(SqliteDataReader r) => new()
    {
        Id = Int(r, "Id"),
        Nombre = (string)r["Nombre"],
        Especialidad = (string)r["Especialidad"],
        Colegiado = (string)r["Colegiado"]
    };

    public List<Veterinario> Listar() =>
        _db.Consultar("SELECT * FROM Veterinarios ORDER BY Nombre;", Mapear);

    public Veterinario? Obtener(int id) =>
        _db.Consultar("SELECT * FROM Veterinarios WHERE Id = $id;", Mapear, P("$id", id)).FirstOrDefault();

    public int Insertar(Veterinario v) => _db.Insertar(
        "INSERT INTO Veterinarios (Nombre, Especialidad, Colegiado) VALUES ($n, $e, $c);",
        P("$n", v.Nombre), P("$e", v.Especialidad), P("$c", v.Colegiado));

    public void Actualizar(Veterinario v) => _db.Ejecutar(
        "UPDATE Veterinarios SET Nombre = $n, Especialidad = $e, Colegiado = $c WHERE Id = $id;",
        P("$n", v.Nombre), P("$e", v.Especialidad), P("$c", v.Colegiado), P("$id", v.Id));

    public void Eliminar(int id) => _db.Ejecutar("DELETE FROM Veterinarios WHERE Id = $id;", P("$id", id));
}
