namespace ClinicaVeterinaria.Dominio;

public class Mascota
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Especie { get; set; } = "";
    public string? Raza { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public string Sexo { get; set; } = "M"; // M = macho, H = hembra
    public int PropietarioId { get; set; }
    public string PropietarioNombre { get; set; } = ""; // se llena con JOIN

    public override string ToString() =>
        $"[{Id}] {Nombre} | {Especie} {Raza ?? ""} | Sexo: {Sexo} | Nac: {FechaNacimiento?.ToString("DD/MM/yyyy") ?? "-"} | Dueño: {PropietarioNombre}";
}
