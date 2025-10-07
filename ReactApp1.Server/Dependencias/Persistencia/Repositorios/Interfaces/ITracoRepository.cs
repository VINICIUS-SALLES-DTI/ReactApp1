using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;

namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios.Interfaces;

public interface ITracoRepository : IRepository<Traco>
{
    Task<IEnumerable<Traco>> GetByNomeAsync(string nome);
    Task<IEnumerable<Traco>> GetByResistenciaAsync(decimal resistenciaMin, decimal resistenciaMax);
    Task<Traco?> GetWithMaterialsAsync(int id);
    Task<IEnumerable<Traco>> GetAllWithMaterialsAsync();

    // Métodos em português para compatibilidade
    Task<Traco?> GetTracoComComponentesAsync(int id);
}
