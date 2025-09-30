namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetPeloIdAsync(int id);
    Task<IEnumerable<T>> GetTodosAsync();
    Task AdicionarAsync(T entity);
    void Atualizar(T entity);
    void Deletar(T entity);
}