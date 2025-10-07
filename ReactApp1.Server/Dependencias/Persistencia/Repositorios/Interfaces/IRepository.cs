using System.Data;

namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios.Interfaces;

public interface IRepository<T> where T : class
{
    // Métodos em inglês (padrão)
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
    void SetTransaction(IDbConnection connection, IDbTransaction transaction);

    // Métodos em português (para compatibilidade com serviços existentes)
    Task<IEnumerable<T>> GetTodosAsync();
    Task<T?> GetPeloIdAsync(int id);
    Task<T> AdicionarAsync(T entity);
    void Atualizar(T entity);
    void Deletar(T entity);
}