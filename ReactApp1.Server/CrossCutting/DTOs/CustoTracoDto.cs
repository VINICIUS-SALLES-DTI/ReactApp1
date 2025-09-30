using System.Globalization;

namespace ReactApp1.Server.CrossCutting.DTOs;


public class CustoTracoDto
{
    public int TracoId { get; set; }
    public string NomeTraco { get; set; } = string.Empty;
    public decimal CustoTotalPorM3 { get; set; }
    
    // Propriedade extra para já enviar o valor formatado como moeda (R$)
    public string CustoFormatado => CustoTotalPorM3.ToString("C", new CultureInfo("pt-BR"));
}
