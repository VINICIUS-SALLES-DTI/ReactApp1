using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;

[Table("tracomaterials")]
public class TracoMaterial
{
    [Key]
    public int TracoId { get; set; }

    [Key]
    public int MaterialId { get; set; }

    public decimal Quantidade { get; set; }
    public string UnidadeMedida { get; set; } = string.Empty;

    // Propriedades de navegação - não mapeadas no banco
    [NotMapped]
    public Traco Traco { get; set; } = null!;

    [NotMapped]
    public Material Material { get; set; } = null!;
}