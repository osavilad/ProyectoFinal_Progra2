using System.Globalization;
using ClinicaVeterinaria.Dominio;
using Microsoft.Data.Sqlite;
using static ClinicaVeterinaria.Persistencia.AccesoDatos;

namespace ClinicaVeterinaria.Persistencia;

internal sealed class RepositorioConsultas
{
    private const string SelectBase =
        @"SELECT c.Id, c.MascotaId, c.VeterinarioId, c.Fecha, c.Motivo, c.Diagnostico, c.Tratamiento, c.Costo,
                 m.Nombre AS MascotaNombre, v.Nombre AS VeterinarioNombre
          FROM Consultas c
          INNER JOIN Mascotas m     ON m.Id = c.MascotaId
          INNER JOIN Veterinarios v ON v.Id = c.VeterinarioId ";

    private readonly AccesoDatos _db;
    public RepositorioConsultas(AccesoDatos db) => _db = db;

    private static Consulta Mapear(SqliteDataReader r) => new()
    {
        Id = Int(r, "Id"),
        MascotaId = Int(r, "MascotaId"),
        VeterinarioId = Int(r, "VeterinarioId"),
        Fecha = Fecha(r, "Fecha") ?? DateTime.Now,
        Motivo = (string)r["Motivo"],
        Diagnostico = Str(r, "Diagnostico"),
        Tratamiento = Str(r, "Tratamiento"),
        Costo = Convert.ToDecimal(r["Costo"], CultureInfo.InvariantCulture),
        MascotaNombre = (string)r["MascotaNombre"],
        VeterinarioNombre = (string)r["VeterinarioNombre"]
    };

    public List<Consulta> Listar() => _db.Consultar(SelectBase + "ORDER BY c.Fecha DESC;", Mapear);

    public List<Consulta> ListarPorMascota(int mascotaId) =>
        _db.Consultar(SelectBase + "WHERE c.MascotaId = $m ORDER BY c.Fecha DESC;", Mapear, P("$m", mascotaId));

    public Consulta? Obtener(int id) =>
        _db.Consultar(SelectBase + "WHERE c.Id = $id;", Mapear, P("$id", id)).FirstOrDefault();

    public int Insertar(Consulta c) => _db.Insertar(
        @"INSERT INTO Consultas (MascotaId, VeterinarioId, Fecha, Motivo, Diagnostico, Tratamiento, Costo)
          VALUES ($m, $v, $f, $mo, $d, $t, $c);",
        P("$m", c.MascotaId), P("$v", c.VeterinarioId), P("$f", c.Fecha.ToString("yyyy-MM-dd HH:mm")),
        P("$mo", c.Motivo), P("$d", c.Diagnostico), P("$t", c.Tratamiento), P("$c", (double)c.Costo));

    public void Actualizar(Consulta c) => _db.Ejecutar(
        @"UPDATE Consultas SET MascotaId = $m, VeterinarioId = $v, Fecha = $f, Motivo = $mo,
          Diagnostico = $d, Tratamiento = $t, Costo = $c WHERE Id = $id;",
        P("$m", c.MascotaId), P("$v", c.VeterinarioId), P("$f", c.Fecha.ToString("yyyy-MM-dd HH:mm")),
        P("$mo", c.Motivo), P("$d", c.Diagnostico), P("$t", c.Tratamiento),
        P("$c", (double)c.Costo), P("$id", c.Id));

    public void Eliminar(int id) => _db.Ejecutar("DELETE FROM Consultas WHERE Id = $id;", P("$id", id));
}
