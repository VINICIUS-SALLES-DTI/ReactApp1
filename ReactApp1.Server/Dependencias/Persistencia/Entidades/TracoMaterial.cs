namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;

public class TracoMaterial
{
    public int TracoId { get; set; }
    public int MaterialId { get; set; }

    public decimal Quantidade { get; set; }
    public string UnidadeMedida { get; set; } = string.Empty;

    public virtual Traco Traco { get; set; } = null!;
    public virtual Material Material { get; set; } = null!;
}