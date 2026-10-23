namespace ClinicaVeterinaria.Dominio;

public class Propietario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string? Email { get; set; }
    public string? Direccion { get; set; }

    public override string ToString() =>
        $"[{Id}] {Nombre} | Tel: {Telefono} | Email: {Email ?? "-"} | Dir: {Direccion ?? "-"}";
}
