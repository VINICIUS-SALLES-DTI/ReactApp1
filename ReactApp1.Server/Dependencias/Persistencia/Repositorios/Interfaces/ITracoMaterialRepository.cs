using System.Data;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;

namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios.Interfaces;

public interface ITracoMaterialRepository
{
    Task<IEnumerable<TracoMaterial>> GetByTracoIdAsync(int tracoId);
    Task<IEnumerable<TracoMaterial>> GetByMaterialIdAsync(int materialId);
    Task<TracoMaterial?> GetByKeysAsync(int tracoId, int materialId);
    Task<TracoMaterial> AddAsync(TracoMaterial tracoMaterial);
    Task<bool> UpdateAsync(TracoMaterial tracoMaterial);
    Task<bool> DeleteAsync(int tracoId, int materialId);
    Task<bool> DeleteByTracoIdAsync(int tracoId);
    Task<bool> ExistsAsync(int tracoId, int materialId);
    void SetTransaction(IDbConnection connection, IDbTransaction transaction);
}