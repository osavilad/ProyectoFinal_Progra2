using ClinicaVeterinaria.Aplicacion;
using ClinicaVeterinaria.Persistencia;

namespace ClinicaVeterinaria;

internal static class Program
{
    private static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        try
        {
            var inicializador = new InicializadorBaseDatos();
            inicializador.Inicializar();

            var db = new AccesoDatos(inicializador.CadenaConexion);
            new MenuClinica(
                new RepositorioPropietarios(db),
                new RepositorioMascotas(db),
                new RepositorioVeterinarios(db),
                new RepositorioConsultas(db)).Ejecutar();

            Console.WriteLine("¡Hasta pronto!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"No fue posible iniciar: {ex.Message}");
        }
    }
}
