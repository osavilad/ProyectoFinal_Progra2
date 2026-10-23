using ClinicaVeterinaria.Dominio;
using Microsoft.Data.Sqlite;
using static ClinicaVeterinaria.Persistencia.AccesoDatos;

namespace ClinicaVeterinaria.Persistencia;

internal sealed class RepositorioMascotas
{
    private const string SelectBase =
        @"SELECT m.Id, m.Nombre, m.Especie, m.Raza, m.FechaNacimiento, m.Sexo, m.PropietarioId,
                 p.Nombre AS PropietarioNombre
          FROM Mascotas m INNER JOIN Propietarios p ON p.Id = m.PropietarioId ";

    private readonly AccesoDatos _db;
    public RepositorioMascotas(AccesoDatos db) => _db = db;

    private static Mascota Mapear(SqliteDataReader r) => new()
    {
        Id = Int(r, "Id"),
        Nombre = (string)r["Nombre"],
        Especie = (string)r["Especie"],
        Raza = Str(r, "Raza"),
        FechaNacimiento = Fecha(r, "FechaNacimiento"),
        Sexo = (string)r["Sexo"],
        PropietarioId = Int(r, "PropietarioId"),
        PropietarioNombre = (string)r["PropietarioNombre"]
    };

    public List<Mascota> Listar() => _db.Consultar(SelectBase + "ORDER BY m.Nombre;", Mapear);

    public Mascota? Obtener(int id) =>
        _db.Consultar(SelectBase + "WHERE m.Id = $id;", Mapear, P("$id", id)).FirstOrDefault();

    public int Insertar(Mascota m) => _db.Insertar(
        @"INSERT INTO Mascotas (Nombre, Especie, Raza, FechaNacimiento, Sexo, PropietarioId)
          VALUES ($n, $e, $r, $f, $s, $p);",
        P("$n", m.Nombre), P("$e", m.Especie), P("$r", m.Raza),
        P("$f", m.FechaNacimiento?.ToString("yyyy-MM-dd")), P("$s", m.Sexo), P("$p", m.PropietarioId));

    public void Actualizar(Mascota m) => _db.Ejecutar(
        @"UPDATE Mascotas SET Nombre = $n, Especie = $e, Raza = $r, FechaNacimiento = $f,
          Sexo = $s, PropietarioId = $p WHERE Id = $id;",
        P("$n", m.Nombre), P("$e", m.Especie), P("$r", m.Raza),
        P("$f", m.FechaNacimiento?.ToString("yyyy-MM-dd")), P("$s", m.Sexo),
        P("$p", m.PropietarioId), P("$id", m.Id));

    public void Eliminar(int id) => _db.Ejecutar("DELETE FROM Mascotas WHERE Id = $id;", P("$id", id));
}
