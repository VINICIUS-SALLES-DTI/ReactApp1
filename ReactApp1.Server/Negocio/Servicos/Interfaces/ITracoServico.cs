using ReactApp1.Server.CrossCutting.DTOs;

namespace ReactApp1.Server.Negocio.Servicos;

public interface ITracoServico
{
    Task<CustoTracoDto?> CalcularCustoPorMetroCubicoAsync(int tracoId);
    Task<TracoDto> CriarAsync(TracoCriacaoDto tracoDto);
    Task<TracoDto?> ObterPorIdAsync(int id);
    Task<TracoDetalhesDto?> ObterDetalhesAsync(int id);
    Task<IEnumerable<TracoDto>> ObterTodosAsync();
    Task<TracoDto?> AtualizarAsync(int id, TracoCriacaoDto tracoDto);
    Task<bool> ExcluirAsync(int id);
    Task<IEnumerable<TracoDto>> PesquisarAsync(string termo);
}
