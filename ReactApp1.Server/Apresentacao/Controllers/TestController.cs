using Microsoft.AspNetCore.Mvc;
using Dapper;
using Npgsql;

[Route("[controller]")]
public class TesteController : ControllerBase
{
  private readonly IConfiguration _configuration;

  public TesteController(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  [HttpGet]
  public IActionResult Get()
  {
    return Ok("API está funcionando");
  }

  [HttpGet("test-connection")]
  public async Task<IActionResult> TestConnection()
  {
    try
    {
      var connectionString = _configuration.GetConnectionString("DefaultConnection");
      using var connection = new NpgsqlConnection(connectionString);
      await connection.OpenAsync();

      var result = await connection.QueryFirstOrDefaultAsync<string>("SELECT 'Connection successful' as message");
      return Ok(new { message = result, status = "success" });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = ex.Message, status = "error" });
    }
  }

  [HttpGet("test-tables")]
  public async Task<IActionResult> TestTables()
  {
    try
    {
      var connectionString = _configuration.GetConnectionString("DefaultConnection");
      using var connection = new NpgsqlConnection(connectionString);
      await connection.OpenAsync();

      var tables = await connection.QueryAsync<string>(@"
                SELECT table_name 
                FROM information_schema.tables 
                WHERE table_schema = 'public' 
                ORDER BY table_name");

      return Ok(new { tables = tables.ToList(), status = "success" });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = ex.Message, status = "error" });
    }
  }

  [HttpGet("test-materials")]
  public async Task<IActionResult> TestMaterials()
  {
    try
    {
      var connectionString = _configuration.GetConnectionString("DefaultConnection");
      using var connection = new NpgsqlConnection(connectionString);
      await connection.OpenAsync();

      // Testar se a tabela Materiais existe e quais colunas tem
      var columns = await connection.QueryAsync<dynamic>(@"
                SELECT column_name, data_type 
                FROM information_schema.columns 
                WHERE table_name = 'Materiais' 
                ORDER BY ordinal_position");

      if (!columns.Any())
      {
        return Ok(new { message = "Table 'Materiais' not found", columns = new List<object>(), status = "warning" });
      }

      // Tentar buscar dados da tabela
      var materials = await connection.QueryAsync<dynamic>("SELECT * FROM \"Materiais\" LIMIT 5");

      return Ok(new
      {
        columns = columns.ToList(),
        materials = materials.ToList(),
        status = "success"
      });
    }
    catch (Exception ex)
    {
      return StatusCode(500, new { message = ex.Message, status = "error" });
    }
  }
}