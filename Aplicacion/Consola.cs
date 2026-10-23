using System.Globalization;

namespace ClinicaVeterinaria.Aplicacion;

/// <summary>Utilidades para leer y validar datos en consola.</summary>
public static class Consola
{
    private static readonly string[] FormatosFecha =
        { "dd/MM/yyyy", "dd/MM/yyyy HH:mm", "d/M/yyyy", "d/M/yyyy H:mm" };

    public static void Titulo(string texto)
    {
        Console.Clear();
        var anterior = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("==============================================");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  {texto}");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("==============================================");
        Console.ForegroundColor = anterior;
    }

    public static void Opcion(string numero, string texto, ConsoleColor color)
    {
        var anterior = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write($"  {numero}. ");
        Console.ForegroundColor = anterior;
        Console.WriteLine(texto);
    }

    public static void Pausa()
    {
        Console.WriteLine();
        Console.Write("Presione ENTER para continuar...");
        Console.ReadLine();
    }

    public static void Info(string msg) => Escribir(msg, ConsoleColor.Green);
    public static void Error(string msg) => Escribir(msg, ConsoleColor.Red);

    private static void Escribir(string msg, ConsoleColor color)
    {
        var anterior = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(msg);
        Console.ForegroundColor = anterior;
    }

    /// <summary>Texto obligatorio. Si se pasa 'actual', ENTER conserva el valor.</summary>
    public static string Texto(string etiqueta, string? actual = null)
    {
        while (true)
        {
            Console.Write(actual is null ? $"{etiqueta}: " : $"{etiqueta} [{actual}]: ");
            var s = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(s)) return s;
            if (actual is not null) return actual;
            Error("  Este campo es obligatorio.");
        }
    }

    /// <summary>Texto opcional. ENTER conserva el valor actual y '-' lo borra.</summary>
    public static string? TextoOpcional(string etiqueta, string? actual = null)
    {
        Console.Write(actual is null ? $"{etiqueta} (opcional): " : $"{etiqueta} [{actual}] (opcional, '-' borra): ");
        var s = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(s)) return actual;
        return s == "-" ? null : s;
    }

    public static int Entero(string etiqueta)
    {
        while (true)
        {
            Console.Write($"{etiqueta}: ");
            if (int.TryParse(Console.ReadLine(), out int n)) return n;
            Error("  Ingrese un número entero válido.");
        }
    }

    public static decimal Decimal(string etiqueta, decimal? actual = null)
    {
        while (true)
        {
            Console.Write(actual is null ? $"{etiqueta}: " : $"{etiqueta} [{actual:N2}]: ");
            var s = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(s) && actual is not null) return actual.Value;
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var d) && d >= 0) return d;
            Error("  Ingrese un monto válido (ej. 150.50).");
        }
    }

    /// <summary>Fecha en formato dd/MM/yyyy (o dd/MM/yyyy HH:mm). Si es opcional, ENTER devuelve el valor actual.</summary>
    public static DateTime? Fecha(string etiqueta, DateTime? actual = null, bool obligatoria = false)
    {
        while (true)
        {
            var sugerido = actual?.ToString("dd/MM/yyyy HH:mm");
            Console.Write(sugerido is null
                ? $"{etiqueta} (dd/MM/yyyy{(obligatoria ? "" : ", opcional")}): "
                : $"{etiqueta} [{sugerido}]: ");
            var s = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(s))
            {
                if (actual is not null || !obligatoria) return actual;
                Error("  La fecha es obligatoria.");
                continue;
            }
            if (DateTime.TryParseExact(s, FormatosFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out var f))
                return f;
            Error("  Formato inválido. Use dd/MM/yyyy o dd/MM/yyyy HH:mm");
        }
    }

    public static string Sexo(string? actual = null)
    {
        while (true)
        {
            Console.Write(actual is null ? "Sexo (M=macho / H=hembra): " : $"Sexo (M/H) [{actual}]: ");
            var s = Console.ReadLine()?.Trim().ToUpperInvariant();
            if (string.IsNullOrEmpty(s) && actual is not null) return actual;
            if (s == "M" || s == "H") return s;
            Error("  Ingrese M o H.");
        }
    }

    public static bool Confirmar(string pregunta)
    {
        Console.Write($"{pregunta} (s/n): ");
        return Console.ReadLine()?.Trim().ToLowerInvariant() == "s";
    }
}
