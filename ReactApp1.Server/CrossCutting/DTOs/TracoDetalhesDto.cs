namespace ReactApp1.Server.CrossCutting.DTOs;

public class TracoDetalhesDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal ResistenciaFck { get; set; }
    public decimal Slump { get; set; }
    public List<ComponenteDto> Componentes { get; set; } = new();
}

public class ComponenteDto
{
    public int MaterialId { get; set; }
    public string NomeMaterial { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public string UnidadeMedida { get; set; } = string.Empty;
}