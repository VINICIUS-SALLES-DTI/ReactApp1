namespace ReactApp1.Server.CrossCutting.DTOs;

public class MaterialDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal PrecoUnitario { get; set; }
    public bool Disponivel { get; set; }
}
