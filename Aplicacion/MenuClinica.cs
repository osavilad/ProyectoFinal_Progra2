using ClinicaVeterinaria.Persistencia;
using ClinicaVeterinaria.Dominio;
using Microsoft.Data.Sqlite;

namespace ClinicaVeterinaria.Aplicacion;

internal sealed class MenuClinica
{
    private readonly RepositorioPropietarios Propietarios;
    private readonly RepositorioMascotas Mascotas;
    private readonly RepositorioVeterinarios Veterinarios;
    private readonly RepositorioConsultas Consultas;

    public MenuClinica(RepositorioPropietarios propietarios, RepositorioMascotas mascotas,
        RepositorioVeterinarios veterinarios, RepositorioConsultas consultas)
    {
        Propietarios = propietarios;
        Mascotas = mascotas;
        Veterinarios = veterinarios;
        Consultas = consultas;
    }

    // ===================== MENÚ PRINCIPAL =====================
    public void Ejecutar()
    {
        while (true)
        {
            Consola.Titulo("SISTEMA DE CONTROL CLÍNICO VETERINARIO");
            Consola.Opcion("1", "Propietarios", ConsoleColor.Cyan);
            Consola.Opcion("2", "Mascotas", ConsoleColor.Green);
            Consola.Opcion("3", "Veterinarios", ConsoleColor.Yellow);
            Consola.Opcion("4", "Consultas", ConsoleColor.Magenta);
            Consola.Opcion("5", "Historial clínico de una mascota", ConsoleColor.Blue);
            Consola.Opcion("0", "Salir", ConsoleColor.Red);
            Console.WriteLine();

            switch (Consola.Entero("Seleccione una opción"))
            {
                case 1: SubMenu("PROPIETARIOS", ListarPropietarios, AgregarPropietario, EditarPropietario, EliminarPropietario); break;
                case 2: SubMenu("MASCOTAS", ListarMascotas, AgregarMascota, EditarMascota, EliminarMascota); break;
                case 3: SubMenu("VETERINARIOS", ListarVeterinarios, AgregarVeterinario, EditarVeterinario, EliminarVeterinario); break;
                case 4: SubMenu("CONSULTAS", ListarConsultas, AgregarConsulta, EditarConsulta, EliminarConsulta); break;
                case 5: HistorialMascota(); break;
                case 0: return;
                default: Consola.Error("Opción no válida."); Consola.Pausa(); break;
            }
        }
    }

    private void SubMenu(string titulo, Action listar, Action agregar, Action editar, Action eliminar)
    {
        while (true)
        {
            Consola.Titulo(titulo);
            Consola.Opcion("1", "Listar", ConsoleColor.Cyan);
            Consola.Opcion("2", "Agregar", ConsoleColor.Green);
            Consola.Opcion("3", "Editar", ConsoleColor.Yellow);
            Consola.Opcion("4", "Eliminar", ConsoleColor.Red);
            Consola.Opcion("0", "Volver", ConsoleColor.Gray);
            Console.WriteLine();

            int op = Consola.Entero("Seleccione una opción");
            if (op == 0) return;

            try
            {
                Console.WriteLine();
                switch (op)
                {
                    case 1: listar(); break;
                    case 2: agregar(); break;
                    case 3: editar(); break;
                    case 4: eliminar(); break;
                    default: Consola.Error("Opción no válida."); break;
                }
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
            {
                Consola.Error("No se puede eliminar: el registro está relacionado con otros datos (mascotas o consultas).");
            }
            catch (SqliteException ex)
            {
                Consola.Error($"Error de base de datos: {ex.Message}");
            }
            Consola.Pausa();
        }
    }

    // ===================== PROPIETARIOS =====================
    private void ListarPropietarios()
    {
        var lista = Propietarios.Listar();
        if (lista.Count == 0) { Console.WriteLine("No hay propietarios registrados."); return; }
        lista.ForEach(Console.WriteLine);
    }

    private void AgregarPropietario()
    {
        var p = new Propietario
        {
            Nombre = Consola.Texto("Nombre completo"),
            Telefono = Consola.Texto("Teléfono"),
            Email = Consola.TextoOpcional("Email"),
            Direccion = Consola.TextoOpcional("Dirección")
        };
        p.Id = Propietarios.Insertar(p);
        Consola.Info($"Propietario registrado con Id {p.Id}.");
    }

    private void EditarPropietario()
    {
        ListarPropietarios();
        var p = Propietarios.Obtener(Consola.Entero("\nId del propietario a editar"));
        if (p is null) { Consola.Error("No existe ese propietario."); return; }

        Console.WriteLine("(ENTER conserva el valor actual)");
        p.Nombre = Consola.Texto("Nombre", p.Nombre);
        p.Telefono = Consola.Texto("Teléfono", p.Telefono);
        p.Email = Consola.TextoOpcional("Email", p.Email);
        p.Direccion = Consola.TextoOpcional("Dirección", p.Direccion);
        Propietarios.Actualizar(p);
        Consola.Info("Propietario actualizado.");
    }

    private void EliminarPropietario()
    {
        ListarPropietarios();
        int id = Consola.Entero("\nId del propietario a eliminar");
        if (Propietarios.Obtener(id) is null) { Consola.Error("No existe ese propietario."); return; }
        if (!Consola.Confirmar("¿Confirma la eliminación?")) return;
        Propietarios.Eliminar(id);
        Consola.Info("Propietario eliminado.");
    }

    // ===================== MASCOTAS =====================
    private void ListarMascotas()
    {
        var lista = Mascotas.Listar();
        if (lista.Count == 0) { Console.WriteLine("No hay mascotas registradas."); return; }
        lista.ForEach(Console.WriteLine);
    }

    private Propietario? ElegirPropietario(int? actualId = null)
    {
        ListarPropietarios();
        if (Propietarios.Listar().Count == 0) { Consola.Error("Primero registre un propietario."); return null; }
        Console.WriteLine();
        var p = Propietarios.Obtener(Consola.Entero("Id del propietario"));
        if (p is null) Consola.Error("No existe ese propietario.");
        return p;
    }

    private void AgregarMascota()
    {
        var dueno = ElegirPropietario();
        if (dueno is null) return;

        var m = new Mascota
        {
            Nombre = Consola.Texto("Nombre de la mascota"),
            Especie = Consola.Texto("Especie (perro, gato, ave...)"),
            Raza = Consola.TextoOpcional("Raza"),
            FechaNacimiento = Consola.Fecha("Fecha de nacimiento"),
            Sexo = Consola.Sexo(),
            PropietarioId = dueno.Id
        };
        m.Id = Mascotas.Insertar(m);
        Consola.Info($"Mascota registrada con Id {m.Id}.");
    }

    private void EditarMascota()
    {
        ListarMascotas();
        var m = Mascotas.Obtener(Consola.Entero("\nId de la mascota a editar"));
        if (m is null) { Consola.Error("No existe esa mascota."); return; }

        Console.WriteLine("(ENTER conserva el valor actual)");
        m.Nombre = Consola.Texto("Nombre", m.Nombre);
        m.Especie = Consola.Texto("Especie", m.Especie);
        m.Raza = Consola.TextoOpcional("Raza", m.Raza);
        m.FechaNacimiento = Consola.Fecha("Fecha de nacimiento", m.FechaNacimiento);
        m.Sexo = Consola.Sexo(m.Sexo);

        if (Consola.Confirmar("¿Cambiar de propietario?"))
        {
            var nuevo = ElegirPropietario();
            if (nuevo is null) return;
            m.PropietarioId = nuevo.Id;
        }
        Mascotas.Actualizar(m);
        Consola.Info("Mascota actualizada.");
    }

    private void EliminarMascota()
    {
        ListarMascotas();
        int id = Consola.Entero("\nId de la mascota a eliminar");
        if (Mascotas.Obtener(id) is null) { Consola.Error("No existe esa mascota."); return; }
        if (!Consola.Confirmar("¿Confirma la eliminación?")) return;
        Mascotas.Eliminar(id);
        Consola.Info("Mascota eliminada.");
    }

    // ===================== VETERINARIOS =====================
    private void ListarVeterinarios()
    {
        var lista = Veterinarios.Listar();
        if (lista.Count == 0) { Console.WriteLine("No hay veterinarios registrados."); return; }
        lista.ForEach(Console.WriteLine);
    }

    private void AgregarVeterinario()
    {
        var v = new Veterinario
        {
            Nombre = Consola.Texto("Nombre completo"),
            Especialidad = Consola.Texto("Especialidad"),
            Colegiado = Consola.Texto("No. de colegiado")
        };
        v.Id = Veterinarios.Insertar(v);
        Consola.Info($"Veterinario registrado con Id {v.Id}.");
    }

    private void EditarVeterinario()
    {
        ListarVeterinarios();
        var v = Veterinarios.Obtener(Consola.Entero("\nId del veterinario a editar"));
        if (v is null) { Consola.Error("No existe ese veterinario."); return; }

        Console.WriteLine("(ENTER conserva el valor actual)");
        v.Nombre = Consola.Texto("Nombre", v.Nombre);
        v.Especialidad = Consola.Texto("Especialidad", v.Especialidad);
        v.Colegiado = Consola.Texto("No. de colegiado", v.Colegiado);
        Veterinarios.Actualizar(v);
        Consola.Info("Veterinario actualizado.");
    }

    private void EliminarVeterinario()
    {
        ListarVeterinarios();
        int id = Consola.Entero("\nId del veterinario a eliminar");
        if (Veterinarios.Obtener(id) is null) { Consola.Error("No existe ese veterinario."); return; }
        if (!Consola.Confirmar("¿Confirma la eliminación?")) return;
        Veterinarios.Eliminar(id);
        Consola.Info("Veterinario eliminado.");
    }

    // ===================== CONSULTAS =====================
    private void ListarConsultas()
    {
        var lista = Consultas.Listar();
        if (lista.Count == 0) { Console.WriteLine("No hay consultas registradas."); return; }
        lista.ForEach(c => { Console.WriteLine(c); Console.WriteLine(); });
    }

    private Mascota? ElegirMascota()
    {
        ListarMascotas();
        if (Mascotas.Listar().Count == 0) { Consola.Error("Primero registre una mascota."); return null; }
        Console.WriteLine();
        var m = Mascotas.Obtener(Consola.Entero("Id de la mascota"));
        if (m is null) Consola.Error("No existe esa mascota.");
        return m;
    }

    private Veterinario? ElegirVeterinario()
    {
        ListarVeterinarios();
        if (Veterinarios.Listar().Count == 0) { Consola.Error("Primero registre un veterinario."); return null; }
        Console.WriteLine();
        var v = Veterinarios.Obtener(Consola.Entero("Id del veterinario"));
        if (v is null) Consola.Error("No existe ese veterinario.");
        return v;
    }

    private void AgregarConsulta()
    {
        var mascota = ElegirMascota();
        if (mascota is null) return;
        Console.WriteLine();
        var vet = ElegirVeterinario();
        if (vet is null) return;
        Console.WriteLine();

        var c = new Consulta
        {
            MascotaId = mascota.Id,
            VeterinarioId = vet.Id,
            Fecha = Consola.Fecha("Fecha y hora (ENTER = ahora)", DateTime.Now, true) ?? DateTime.Now,
            Motivo = Consola.Texto("Motivo de la consulta"),
            Diagnostico = Consola.TextoOpcional("Diagnóstico"),
            Tratamiento = Consola.TextoOpcional("Tratamiento"),
            Costo = Consola.Decimal("Costo (Q)")
        };
        c.Id = Consultas.Insertar(c);
        Consola.Info($"Consulta registrada con Id {c.Id}.");
    }

    private void EditarConsulta()
    {
        ListarConsultas();
        var c = Consultas.Obtener(Consola.Entero("\nId de la consulta a editar"));
        if (c is null) { Consola.Error("No existe esa consulta."); return; }

        Console.WriteLine("(ENTER conserva el valor actual)");
        c.Fecha = Consola.Fecha("Fecha y hora", c.Fecha, true) ?? c.Fecha;
        c.Motivo = Consola.Texto("Motivo", c.Motivo);
        c.Diagnostico = Consola.TextoOpcional("Diagnóstico", c.Diagnostico);
        c.Tratamiento = Consola.TextoOpcional("Tratamiento", c.Tratamiento);
        c.Costo = Consola.Decimal("Costo (Q)", c.Costo);
        Consultas.Actualizar(c);
        Consola.Info("Consulta actualizada.");
    }

    private void EliminarConsulta()
    {
        ListarConsultas();
        int id = Consola.Entero("\nId de la consulta a eliminar");
        if (Consultas.Obtener(id) is null) { Consola.Error("No existe esa consulta."); return; }
        if (!Consola.Confirmar("¿Confirma la eliminación?")) return;
        Consultas.Eliminar(id);
        Consola.Info("Consulta eliminada.");
    }

    // ===================== HISTORIAL CLÍNICO =====================
    private void HistorialMascota()
    {
        Consola.Titulo("HISTORIAL CLÍNICO");
        try
        {
            var m = ElegirMascota();
            if (m is not null)
            {
                var consultas = Consultas.ListarPorMascota(m.Id);
                Console.WriteLine($"\nHistorial de {m.Nombre} ({m.Especie}) - Dueño: {m.PropietarioNombre}");
                Console.WriteLine("----------------------------------------------");
                if (consultas.Count == 0) Console.WriteLine("Sin consultas registradas.");
                else
                {
                    consultas.ForEach(c => { Console.WriteLine(c); Console.WriteLine(); });
                    Console.WriteLine($"Total de consultas: {consultas.Count} | Total facturado: Q{consultas.Sum(c => c.Costo):N2}");
                }
            }
        }
        catch (SqliteException ex) { Consola.Error($"Error de base de datos: {ex.Message}"); }
        Consola.Pausa();
    }
}
