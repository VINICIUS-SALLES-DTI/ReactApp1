namespace ReactApp1.Server.CrossCutting.DTOs;

public class TracoCriacaoDto
{
    public string Nome { get; set; } = string.Empty;
    public decimal ResistenciaFck { get; set; }
    public decimal Slump { get; set; }
    public List<ComponenteCriacaoDto> Componentes { get; set; } = new();
}

public class ComponenteCriacaoDto
{
    public int MaterialId { get; set; }
    public decimal Quantidade { get; set; }
    public string UnidadeMedida { get; set; } = string.Empty;
}