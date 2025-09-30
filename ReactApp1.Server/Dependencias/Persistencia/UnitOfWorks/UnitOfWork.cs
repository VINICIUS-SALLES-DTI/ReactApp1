using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios.Interfaces;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.UnitOfWorks.Interfaces;
namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.UnitOfWorks;

public class UnitOfWork : IUnitOfWork
{
    private readonly SuperMixDbContext _context;
    public IMaterialRepository Materiais { get; private set; }
    public ITracoRepository Tracos { get; private set; }

    public UnitOfWork(SuperMixDbContext context)
    {
        _context = context;
        Materiais = new MaterialRepository(_context);
        Tracos = new TracoRepository(_context);
    }

    public async Task<int> CompletarAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
