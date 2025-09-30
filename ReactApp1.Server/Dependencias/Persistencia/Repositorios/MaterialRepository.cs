using System;
using Microsoft.EntityFrameworkCore;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios.Interfaces;

namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios;

public class MaterialRepository : Repository<Material>, IMaterialRepository
{
    public MaterialRepository(SuperMixDbContext context) : base(context)
    {
    }
    
    public async Task<Material?> GetPeloNomeAsync(string nome)
    {
        return await _context.Set<Material>().FirstOrDefaultAsync(m => m.Nome == nome);
    }
}