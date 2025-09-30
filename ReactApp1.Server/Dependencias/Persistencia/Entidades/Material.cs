namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;

public class Material
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal PrecoUnitario { get; set; }
    public bool Disponivel { get; set; } = true;
    public virtual ICollection<TracoMaterial> TracoMateriais { get; set; } = new List<TracoMaterial>();
}