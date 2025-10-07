using Dommel;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;

namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Configuracao;

public static class DommelConfig
{
    public static void Configure()
    {
        // Configuração básica para PostgreSQL
        // O Dommel irá usar os atributos [Table] das entidades para mapear os nomes das tabelas

        // Para entidades com chaves compostas, podemos configurar manualmente se necessário
        // O TracoMaterial tem chave composta (TracoId, MaterialId)
    }
}