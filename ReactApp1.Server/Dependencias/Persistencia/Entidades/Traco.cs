using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;

[Table("tracos")]
public class Traco
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 100 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Resistência Fck é obrigatória")]
    [Range(0, double.MaxValue, ErrorMessage = "Resistência Fck deve estar entre 0 e 100 MPa")]
    public decimal ResistenciaFck { get; set; } // em MPa

    [Required(ErrorMessage = "Slump é obrigatório")]
    [Range(0, double.MaxValue, ErrorMessage = "Slump deve ser maior ou igual a 0 cm")]
    public decimal Slump { get; set; } // em cm

    [NotMapped]
    public ICollection<TracoMaterial> TracoMateriais { get; set; } = new List<TracoMaterial>();
}
