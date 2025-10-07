using System.Data;
using Dapper;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios.Interfaces;

namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios;

public class MaterialRepository : Repository<Material>, IMaterialRepository
{
    public MaterialRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<IEnumerable<Material>> GetByNomeAsync(string nome)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM materials WHERE nome ILIKE @Nome";
        return await connection.QueryAsync<Material>(sql, new { Nome = $"%{nome}%" }, transaction: _transaction);
    }

    public async Task<IEnumerable<Material>> GetDisponivelAsync()
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM materials WHERE disponivel = true";
        return await connection.QueryAsync<Material>(sql, transaction: _transaction);
    }

    public async Task<bool> SetDisponibilidadeAsync(int id, bool disponivel)
    {
        using var connection = CreateConnection();
        var sql = "UPDATE materials SET disponivel = @Disponivel WHERE id = @Id";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, Disponivel = disponivel }, transaction: _transaction);
        return rowsAffected > 0;
    }

    protected override string GetTableName()
    {
        return "materials";
    }
}