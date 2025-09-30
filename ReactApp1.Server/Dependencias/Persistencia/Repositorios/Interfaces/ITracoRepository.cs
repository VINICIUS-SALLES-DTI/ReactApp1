namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios.Interfaces;

public interface ITracoRepository : IRepository<Entidades.Traco>
{
    Task<Entidades.Traco?> GetTracoComComponentesAsync(int tracoId);
}
