namespace ClinicaVeterinaria.Dominio;

public class Veterinario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Especialidad { get; set; } = "";
    public string Colegiado { get; set; } = "";

    public override string ToString() =>
        $"[{Id}] Dr(a). {Nombre} | {Especialidad} | Colegiado: {Colegiado}";
}
