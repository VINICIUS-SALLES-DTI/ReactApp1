using Microsoft.AspNetCore.Mvc;
using ReactApp1.Server.CrossCutting.DTOs;
using ReactApp1.Server.Negocio.Servicos.Interfaces;

namespace ReactApp1.Server.Apresentacao.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialServico _materialServico;

        // Injeção de Dependência: O serviço é injetado no construtor
        public MaterialController(IMaterialServico materialServico)
        {
            _materialServico = materialServico;
        }

        // GET: api/materiais
        [HttpGet]
        public async Task<IActionResult> ObterTodosAsync()
        {
            try
            {
                var materiais = await _materialServico.ObterTodosAsync();
                return Ok(materiais); // Retorna 200 OK com a lista de materiais
            }
            catch (Exception)
            {
                // É uma boa prática logar o erro aqui
                return StatusCode(500, "Ocorreu um erro interno ao buscar os materiais.");
            }
        }

        // GET: api/materiais/5
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorIdAsync(int id)
        {
            var material = await _materialServico.ObterPorIdAsync(id);
            if (material == null)
            {
                return NotFound(); // Retorna 404 Not Found se o material não existir
            }
            return Ok(material);
        }

        // POST: api/materiais
        [HttpPost]
        public async Task<IActionResult> CriarAsync([FromBody] MaterialCriacaoDto materialDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Retorna 400 Bad Request se o DTO for inválido
            }
            try
            {
                var novoMaterial = await _materialServico.CriarAsync(materialDto);
                var location = Url.Action(nameof(ObterPorIdAsync), new { id = novoMaterial.Id });
                return Created(location, novoMaterial);
            }
            catch (Exception ex)
            {
                // É uma boa prática logar o erro aqui
                return StatusCode(500, $"Ocorreu um erro interno ao criar o material: {ex.Message}");
            }

        }

        // PUT: api/materiais/5
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarAsync(int id, [FromBody] MaterialCriacaoDto materialDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Retorna 400 Bad Request se o DTO for inválido
            }
            try
            {
                var atualizado = await _materialServico.AtualizarAsync(id, materialDto);
                if (!atualizado)
                {
                    return NotFound(); // Retorna 404 Not Found se o material não existir
                }
                return NoContent(); // Retorna 204 No Content para indicar sucesso sem corpo
            }
            catch (Exception ex)
            {
                // É uma boa prática logar o erro aqui
                return StatusCode(500, $"Ocorreu um erro interno ao atualizar o material: {ex.Message}");
            }
        }

        // DELETE: api/materiais/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> ExcluirAsync(int id)
        {
            try
            {
                var deletado = await _materialServico.ExcluirAsync(id);
                if (!deletado)
                {
                    return NotFound(); // Retorna 404 Not Found se o material não existir
                }
                return NoContent(); // Retorna 204 No Content para indicar sucesso sem corpo
            }
            catch (Exception ex)
            {
                // É uma boa prática logar o erro aqui
                return StatusCode(500, $"Ocorreu um erro interno ao deletar o material: {ex.Message}");
            }
        }

        // GET: api/materiais/pesquisar?termo=cimento
        [HttpGet("pesquisar")]
        public async Task<IActionResult> PesquisarAsync([FromQuery] string termo)
        {
            try
            {
                var resultados = await _materialServico.PesquisarAsync(termo);
                return Ok(resultados); // Retorna 200 OK com os resultados da pesquisa
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno ao pesquisar os materiais.");
            }
        }
    }
}