using System;
using ReactApp1.Server.CrossCutting.DTOs;

namespace ReactApp1.Server.Negocio.Servicos.Interfaces;

public interface IMaterialServico
{
    Task<IEnumerable<MaterialDto>> ObterTodosAsync();
    Task<MaterialDto?> ObterPorIdAsync(int id);
    Task<MaterialDto> CriarAsync(MaterialCriacaoDto materialDto);
    Task<bool> AtualizarAsync(int id, MaterialCriacaoDto materialDto);
    Task<bool> ExcluirAsync(int id);
    Task<IEnumerable<MaterialDto>> PesquisarAsync(string termo);
}