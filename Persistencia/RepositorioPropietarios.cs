using ClinicaVeterinaria.Dominio;
using Microsoft.Data.Sqlite;
using static ClinicaVeterinaria.Persistencia.AccesoDatos;

namespace ClinicaVeterinaria.Persistencia;

internal sealed class RepositorioPropietarios
{
    private readonly AccesoDatos _db;
    public RepositorioPropietarios(AccesoDatos db) => _db = db;

    private static Propietario Mapear(SqliteDataReader r) => new()
    {
        Id = Int(r, "Id"),
        Nombre = (string)r["Nombre"],
        Telefono = (string)r["Telefono"],
        Email = Str(r, "Email"),
        Direccion = Str(r, "Direccion")
    };

    public List<Propietario> Listar() =>
        _db.Consultar("SELECT * FROM Propietarios ORDER BY Nombre;", Mapear);

    public Propietario? Obtener(int id) =>
        _db.Consultar("SELECT * FROM Propietarios WHERE Id = $id;", Mapear, P("$id", id)).FirstOrDefault();

    public int Insertar(Propietario p) => _db.Insertar(
        "INSERT INTO Propietarios (Nombre, Telefono, Email, Direccion) VALUES ($n, $t, $e, $d);",
        P("$n", p.Nombre), P("$t", p.Telefono), P("$e", p.Email), P("$d", p.Direccion));

    public void Actualizar(Propietario p) => _db.Ejecutar(
        "UPDATE Propietarios SET Nombre = $n, Telefono = $t, Email = $e, Direccion = $d WHERE Id = $id;",
        P("$n", p.Nombre), P("$t", p.Telefono), P("$e", p.Email), P("$d", p.Direccion), P("$id", p.Id));

    public void Eliminar(int id) => _db.Ejecutar("DELETE FROM Propietarios WHERE Id = $id;", P("$id", id));
}
