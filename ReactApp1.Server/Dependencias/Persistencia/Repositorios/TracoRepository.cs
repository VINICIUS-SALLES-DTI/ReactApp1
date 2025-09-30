using System;
using Microsoft.EntityFrameworkCore;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios.Interfaces;

namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios;


public class TracoRepository : Repository<Traco>, ITracoRepository
{
    public TracoRepository(SuperMixDbContext context) : base(context)
    {
    }

    public async Task<Traco?> GetTracoComComponentesAsync(int tracoId)
    {
        return await _context.Tracos
            .Include(t => t.TracoMateriais) // Inclui a tabela de junção
            .ThenInclude(tm => tm.Material) // A partir da junção, inclui o Material
            .FirstOrDefaultAsync(t => t.Id == tracoId);
    }
}
