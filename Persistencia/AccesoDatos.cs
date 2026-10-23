using Microsoft.Data.Sqlite;

namespace ClinicaVeterinaria.Persistencia;

/// <summary>Ejecuta SQL con parámetros sobre la base SQLite local (una conexión por operación).</summary>
internal sealed class AccesoDatos
{
    private readonly string _cadenaConexion;

    public AccesoDatos(string cadenaConexion) => _cadenaConexion = cadenaConexion;

    public static (string Nombre, object? Valor) P(string nombre, object? valor) => (nombre, valor);

    private SqliteConnection Abrir()
    {
        var conexion = new SqliteConnection(_cadenaConexion);
        conexion.Open();
        return conexion;
    }

    private static void Agregar(SqliteCommand comando, (string Nombre, object? Valor)[] parametros)
    {
        foreach (var (nombre, valor) in parametros)
            comando.Parameters.AddWithValue(nombre, valor ?? DBNull.Value);
    }

    public int Ejecutar(string sql, params (string Nombre, object? Valor)[] parametros)
    {
        using var conexion = Abrir();
        using var comando = conexion.CreateCommand();
        comando.CommandText = sql;
        Agregar(comando, parametros);
        return comando.ExecuteNonQuery();
    }

    /// <summary>Ejecuta un INSERT y devuelve el Id generado.</summary>
    public int Insertar(string sql, params (string Nombre, object? Valor)[] parametros)
    {
        using var conexion = Abrir();
        using (var insertar = conexion.CreateCommand())
        {
            insertar.CommandText = sql;
            Agregar(insertar, parametros);
            insertar.ExecuteNonQuery();
        }
        using var id = conexion.CreateCommand();
        id.CommandText = "SELECT last_insert_rowid();";
        return Convert.ToInt32(id.ExecuteScalar());
    }

    public List<T> Consultar<T>(string sql, Func<SqliteDataReader, T> mapear,
        params (string Nombre, object? Valor)[] parametros)
    {
        var lista = new List<T>();
        using var conexion = Abrir();
        using var comando = conexion.CreateCommand();
        comando.CommandText = sql;
        Agregar(comando, parametros);
        using var lector = comando.ExecuteReader();
        while (lector.Read()) lista.Add(mapear(lector));
        return lista;
    }

    public static int Int(SqliteDataReader r, string col) => Convert.ToInt32(r[col]);

    public static string? Str(SqliteDataReader r, string col) =>
        r[col] is DBNull ? null : (string)r[col];

    public static DateTime? Fecha(SqliteDataReader r, string col) =>
        r[col] is DBNull
            ? null
            : DateTime.Parse((string)r[col], System.Globalization.CultureInfo.InvariantCulture);
}
