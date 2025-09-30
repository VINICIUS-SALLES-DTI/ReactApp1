using Microsoft.AspNetCore.Mvc;
using ReactApp1.Server.CrossCutting.DTOs;
using ReactApp1.Server.Negocio.Servicos;

namespace ReactApp1.Server.Apresentacao.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TracoController : ControllerBase
    {
        private readonly ITracoServico _tracoServico;

        public TracoController(ITracoServico tracoServico)
        {
            _tracoServico = tracoServico;
        }

        // GET: api/tracos
        [HttpGet]
        public async Task<IActionResult> ObterTodosAsync()
        {
            var tracos = await _tracoServico.ObterTodosAsync();
            return Ok(tracos);
        }

        // GET: api/tracos/detalhes/5
        [HttpGet("detalhes/{id}")]
        public async Task<IActionResult> ObterDetalhesAsync(int id)
        {
            var traco = await _tracoServico.ObterDetalhesAsync(id);
            if (traco == null)
            {
                return NotFound($"Traço com ID {id} não encontrado.");
            }
            return Ok(traco);
        }

        // POST: api/tracos
        [HttpPost]
        public async Task<IActionResult> CriarAsync([FromBody] TracoCriacaoDto tracoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var novoTraco = await _tracoServico.CriarAsync(tracoDto);
                var location = Url.Action(nameof(ObterDetalhesAsync), new { id = novoTraco.Id });
                return Created(location, novoTraco);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar o traço: {ex.Message}");
            }
        }

        // GET: api/tracos/5/custo
        [HttpGet("{id}/custo")]
        public async Task<IActionResult> CalcularCusto(int id)
        {
            try
            {
                var custoDto = await _tracoServico.CalcularCustoPorMetroCubicoAsync(id);
                if (custoDto == null)
                {
                    return NotFound($"Traço com ID {id} não encontrado para cálculo.");
                }
                return Ok(custoDto); // Retorna 200 OK com o DTO de custo
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao calcular o custo: {ex.Message}");
            }
        }

        //GET: api/tracos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorIdAsync(int id)
        {
            var traco = await _tracoServico.ObterPorIdAsync(id);
            if (traco == null)
            {
                return NotFound($"Traço com ID {id} não encontrado.");
            }
            return Ok(traco);
        }

        // PUT: api/tracos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarAsync(int id, [FromBody] TracoCriacaoDto tracoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var tracoAtualizado = await _tracoServico.AtualizarAsync(id, tracoDto);
                if (tracoAtualizado == null)
                {
                    return NotFound($"Traço com ID {id} não encontrado para atualização.");
                }
                return Ok(tracoAtualizado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar o traço: {ex.Message}");
            }
        }

        // DELETE: api/tracos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> ExcluirAsync(int id)
        {
            try
            {
                var sucesso = await _tracoServico.ExcluirAsync(id);
                if (!sucesso)
                {
                    return NotFound($"Traço com ID {id} não encontrado para exclusão.");
                }
                return NoContent(); // Retorna 204 No Content em caso de sucesso
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao excluir o traço: {ex.Message}");
            }
        }

        // GET: api/tracos/pesquisar?termo=concreto
        [HttpGet("pesquisar")]
        public async Task<IActionResult> PesquisarAsync([FromQuery] string termo)
        {
            try
            {
                var resultados = await _tracoServico.PesquisarAsync(termo);
                return Ok(resultados); // Retorna 200 OK com os resultados da pesquisa
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno ao pesquisar os traços.");
            }
        }
    }
}
