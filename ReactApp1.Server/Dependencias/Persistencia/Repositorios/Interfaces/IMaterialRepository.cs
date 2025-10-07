using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;

namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios.Interfaces;

public interface IMaterialRepository : IRepository<Material>
{
    Task<IEnumerable<Material>> GetByNomeAsync(string nome);
    Task<IEnumerable<Material>> GetDisponivelAsync();
    Task<bool> SetDisponibilidadeAsync(int id, bool disponivel);
}