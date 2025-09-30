namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;

public class Traco
{
    public int Id { get; set; }
    
    public string Nome { get; set; } = string.Empty;
    public decimal ResistenciaFck { get; set; } // em MPa

    public decimal Slump { get; set; } // em cm

    public virtual ICollection<TracoMaterial> TracoMateriais { get; set; } = new List<TracoMaterial>();
}
