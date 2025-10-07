using System.Data;
using Dapper;
using Npgsql;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios.Interfaces;

namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios;

public class TracoMaterialRepository : ITracoMaterialRepository
{
    private readonly string _connectionString;
    protected IDbConnection? _sharedConnection;
    protected IDbTransaction? _transaction;

    public TracoMaterialRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private IDbConnection CreateConnection()
    {
        return _sharedConnection ?? new NpgsqlConnection(_connectionString);
    }

    public void SetTransaction(IDbConnection connection, IDbTransaction transaction)
    {
        _sharedConnection = connection;
        _transaction = transaction;
    }

    public async Task<IEnumerable<TracoMaterial>> GetByTracoIdAsync(int tracoId)
    {
        using var connection = CreateConnection();
        var sql = @"
            SELECT tm.*, m.*
            FROM ""TracoMateriais"" tm
            INNER JOIN ""Materiais"" m ON tm.""MaterialId"" = m.""Id""
            WHERE tm.""TracoId"" = @TracoId";

        return await connection.QueryAsync<TracoMaterial, Material, TracoMaterial>(
            sql,
            (tracoMaterial, material) =>
            {
                tracoMaterial.Material = material;
                return tracoMaterial;
            },
            new { TracoId = tracoId },
            transaction: _transaction,
            splitOn: "id");
    }

    public async Task<IEnumerable<TracoMaterial>> GetByMaterialIdAsync(int materialId)
    {
        using var connection = CreateConnection();
        var sql = @"
            SELECT tm.*, t.*
            FROM ""TracoMateriais"" tm
            INNER JOIN ""Tracos"" t ON tm.""TracoId"" = t.""Id""
            WHERE tm.""MaterialId"" = @MaterialId";

        return await connection.QueryAsync<TracoMaterial, Traco, TracoMaterial>(
            sql,
            (tracoMaterial, traco) =>
            {
                tracoMaterial.Traco = traco;
                return tracoMaterial;
            },
            new { MaterialId = materialId },
            transaction: _transaction,
            splitOn: "id");
    }

    public async Task<TracoMaterial?> GetByKeysAsync(int tracoId, int materialId)
    {
        using var connection = CreateConnection();
        var sql = @"
            SELECT tm.*, t.*, m.*
            FROM ""TracoMateriais"" tm
            INNER JOIN ""Tracos"" t ON tm.""TracoId"" = t.""Id""
            INNER JOIN ""Materiais"" m ON tm.""MaterialId"" = m.""Id""
            WHERE tm.""TracoId"" = @TracoId AND tm.""MaterialId"" = @MaterialId";

        var result = await connection.QueryAsync<TracoMaterial, Traco, Material, TracoMaterial>(
            sql,
            (tracoMaterial, traco, material) =>
            {
                tracoMaterial.Traco = traco;
                tracoMaterial.Material = material;
                return tracoMaterial;
            },
            new { TracoId = tracoId, MaterialId = materialId },
            transaction: _transaction,
            splitOn: "id,id");

        return result.FirstOrDefault();
    }

    public async Task<TracoMaterial> AddAsync(TracoMaterial tracoMaterial)
    {
        using var connection = CreateConnection();
        var sql = @"
            INSERT INTO ""TracoMateriais"" (""TracoId"", ""MaterialId"", ""Quantidade"", ""UnidadeMedida"")
            VALUES (@TracoId, @MaterialId, @Quantidade, @UnidadeMedida)";

        await connection.ExecuteAsync(sql, tracoMaterial, transaction: _transaction);
        return tracoMaterial;
    }

    public async Task<bool> UpdateAsync(TracoMaterial tracoMaterial)
    {
        using var connection = CreateConnection();
        var sql = @"
            UPDATE ""TracoMateriais"" 
            SET ""Quantidade"" = @Quantidade, ""UnidadeMedida"" = @UnidadeMedida
            WHERE ""TracoId"" = @TracoId AND ""MaterialId"" = @MaterialId";

        var rowsAffected = await connection.ExecuteAsync(sql, tracoMaterial, transaction: _transaction);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int tracoId, int materialId)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM \"TracoMateriais\" WHERE \"TracoId\" = @TracoId AND \"MaterialId\" = @MaterialId";
        var rowsAffected = await connection.ExecuteAsync(sql, new { TracoId = tracoId, MaterialId = materialId }, transaction: _transaction);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteByTracoIdAsync(int tracoId)
    {
        using var connection = CreateConnection();
        var sql = "DELETE FROM \"TracoMateriais\" WHERE \"TracoId\" = @TracoId";
        var rowsAffected = await connection.ExecuteAsync(sql, new { TracoId = tracoId }, transaction: _transaction);
        return rowsAffected > 0;
    }

    public async Task<bool> ExistsAsync(int tracoId, int materialId)
    {
        using var connection = CreateConnection();
        var sql = "SELECT COUNT(1) FROM \"TracoMateriais\" WHERE \"TracoId\" = @TracoId AND \"MaterialId\" = @MaterialId";
        var count = await connection.QuerySingleAsync<int>(sql, new { TracoId = tracoId, MaterialId = materialId }, transaction: _transaction);
        return count > 0;
    }
}