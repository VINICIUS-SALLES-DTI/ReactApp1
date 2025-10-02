using System.ComponentModel.DataAnnotations;


namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;

public class Material
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Preço unitário é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Preço unitário deve ser maior que zero")]
    public decimal PrecoUnitario { get; set; }

    public bool Disponivel { get; set; } = true;

    public virtual ICollection<TracoMaterial> TracoMateriais { get; set; } = new List<TracoMaterial>();
}