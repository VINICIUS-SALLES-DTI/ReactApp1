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

    public async Task<IEnumerable<Material>> GetPeloNomeAsync(string nome)
    {
        var connection = CreateConnection();
        var shouldDispose = _sharedConnection == null;

        try
        {
            if (shouldDispose)
                await ((Npgsql.NpgsqlConnection)connection).OpenAsync();

            var sql = "SELECT * FROM \"Materiais\" WHERE \"Nome\" ILIKE @Nome";
            return await connection.QueryAsync<Material>(sql, new { Nome = $"%{nome}%" }, transaction: _transaction);
        }
        finally
        {
            if (shouldDispose)
                connection.Dispose();
        }
    }

    public async Task<IEnumerable<Material>> GetDisponivelAsync()
    {
        var connection = CreateConnection();
        var shouldDispose = _sharedConnection == null;

        try
        {
            if (shouldDispose)
                await ((Npgsql.NpgsqlConnection)connection).OpenAsync();

            var sql = "SELECT * FROM \"Materiais\" WHERE \"Disponivel\" = true";
            return await connection.QueryAsync<Material>(sql, transaction: _transaction);
        }
        finally
        {
            if (shouldDispose)
                connection.Dispose();
        }
    }

    public async Task<bool> SetDisponibilidadeAsync(int id, bool disponivel)
    {
        var connection = CreateConnection();
        var shouldDispose = _sharedConnection == null;

        try
        {
            if (shouldDispose)
                await ((Npgsql.NpgsqlConnection)connection).OpenAsync();

            var sql = "UPDATE \"Materiais\" SET \"Disponivel\" = @Disponivel WHERE \"Id\" = @Id";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, Disponivel = disponivel }, transaction: _transaction);
            return rowsAffected > 0;
        }
        finally
        {
            if (shouldDispose)
                connection.Dispose();
        }
    }

    protected override string GetTableName()
    {
        return "\"Materiais\"";
    }
}