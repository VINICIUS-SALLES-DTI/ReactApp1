
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios.Interfaces;

namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.UnitOfWorks.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IMaterialRepository Materiais { get; }
    ITracoRepository Tracos { get; }
    Task<int> CompletarAsync();
}
