namespace ClinicaVeterinaria.Dominio;

public class Consulta
{
    public int Id { get; set; }
    public int MascotaId { get; set; }
    public int VeterinarioId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string Motivo { get; set; } = "";
    public string? Diagnostico { get; set; }
    public string? Tratamiento { get; set; }
    public decimal Costo { get; set; }
    public string MascotaNombre { get; set; } = "";      // se llena con JOIN
    public string VeterinarioNombre { get; set; } = "";  // se llena con JOIN

    public override string ToString() =>
        $"[{Id}] {Fecha:dd/MM/yyyy HH:mm} | Mascota: {MascotaNombre} | Vet: {VeterinarioNombre}\n" +
        $"     Motivo: {Motivo}\n" +
        $"     Diagnóstico: {Diagnostico ?? "-"}\n" +
        $"     Tratamiento: {Tratamiento ?? "-"}\n" +
        $"     Costo: Q{Costo:N2}";
}
