using Microsoft.EntityFrameworkCore;
using ReactApp1.Server.Apresentacao.Dependencias;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios.Interfaces;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly SuperMixDbContext _context;

    public Repository(SuperMixDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
    }

    public void Deletar(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public async Task<IEnumerable<T>> GetTodosAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T?> GetPeloIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public void Atualizar(T entity)
    {
        _context.Set<T>().Update(entity);
    }
}