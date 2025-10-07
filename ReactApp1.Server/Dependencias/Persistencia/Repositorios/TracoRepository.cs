using System.Data;
using Dapper;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios.Interfaces;

namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios;

public class TracoRepository : Repository<Traco>, ITracoRepository
{
    public TracoRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<IEnumerable<Traco>> GetByNomeAsync(string nome)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM tracos WHERE nome ILIKE @Nome";
        return await connection.QueryAsync<Traco>(sql, new { Nome = $"%{nome}%" }, transaction: _transaction);
    }

    public async Task<IEnumerable<Traco>> GetByResistenciaAsync(decimal resistenciaMin, decimal resistenciaMax)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM tracos WHERE resistenciafck BETWEEN @ResistenciaMin AND @ResistenciaMax";
        return await connection.QueryAsync<Traco>(sql, new { ResistenciaMin = resistenciaMin, ResistenciaMax = resistenciaMax }, transaction: _transaction);
    }

    public async Task<Traco?> GetWithMaterialsAsync(int id)
    {
        using var connection = CreateConnection();

        var sql = @"
            SELECT t.*, tm.*, m.*
            FROM tracos t
            LEFT JOIN tracomaterials tm ON t.id = tm.tracoid
            LEFT JOIN materials m ON tm.materialid = m.id
            WHERE t.id = @Id";

        var tracoDictionary = new Dictionary<int, Traco>();

        var result = await connection.QueryAsync<Traco, TracoMaterial, Material, Traco>(
            sql,
            (traco, tracoMaterial, material) =>
            {
                if (!tracoDictionary.TryGetValue(traco.Id, out var tracoEntry))
                {
                    tracoEntry = traco;
                    tracoEntry.TracoMateriais = new List<TracoMaterial>();
                    tracoDictionary.Add(traco.Id, tracoEntry);
                }

                if (tracoMaterial != null && material != null)
                {
                    tracoMaterial.Material = material;
                    tracoEntry.TracoMateriais.Add(tracoMaterial);
                }

                return tracoEntry;
            },
            new { Id = id },
            transaction: _transaction,
            splitOn: "tracoid,id");

        return tracoDictionary.Values.FirstOrDefault();
    }

    public async Task<IEnumerable<Traco>> GetAllWithMaterialsAsync()
    {
        using var connection = CreateConnection();

        var sql = @"
            SELECT t.*, tm.*, m.*
            FROM tracos t
            LEFT JOIN tracomaterials tm ON t.id = tm.tracoid
            LEFT JOIN materials m ON tm.materialid = m.id
            ORDER BY t.id";

        var tracoDictionary = new Dictionary<int, Traco>();

        await connection.QueryAsync<Traco, TracoMaterial, Material, Traco>(
            sql,
            (traco, tracoMaterial, material) =>
            {
                if (!tracoDictionary.TryGetValue(traco.Id, out var tracoEntry))
                {
                    tracoEntry = traco;
                    tracoEntry.TracoMateriais = new List<TracoMaterial>();
                    tracoDictionary.Add(traco.Id, tracoEntry);
                }

                if (tracoMaterial != null && material != null)
                {
                    tracoMaterial.Material = material;
                    tracoEntry.TracoMateriais.Add(tracoMaterial);
                }

                return tracoEntry;
            },
            transaction: _transaction,
            splitOn: "tracoid,id");

        return tracoDictionary.Values;
    }

    protected override string GetTableName()
    {
        return "tracos";
    }

    // Método em português para compatibilidade
    public async Task<Traco?> GetTracoComComponentesAsync(int id)
    {
        return await GetWithMaterialsAsync(id);
    }
}
