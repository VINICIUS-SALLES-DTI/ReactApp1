using System.Data;
using Dapper;
using Dommel;
using Npgsql;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios.Interfaces;

namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios;

public abstract class Repository<T> : IRepository<T> where T : class
{
    protected readonly string _connectionString;
    protected IDbConnection? _sharedConnection;
    protected IDbTransaction? _transaction;

    protected Repository(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected IDbConnection CreateConnection()
    {
        if (_sharedConnection != null)
            return _sharedConnection;
        return new NpgsqlConnection(_connectionString);
    }

    public void SetTransaction(IDbConnection connection, IDbTransaction transaction)
    {
        _sharedConnection = connection;
        _transaction = transaction;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        var connection = CreateConnection();
        var shouldDispose = _sharedConnection == null;

        try
        {
            if (shouldDispose)
                await ((NpgsqlConnection)connection).OpenAsync();

            return await connection.GetAllAsync<T>(transaction: _transaction);
        }
        finally
        {
            if (shouldDispose)
                connection.Dispose();
        }
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        var connection = CreateConnection();
        var shouldDispose = _sharedConnection == null;

        try
        {
            if (shouldDispose)
                await ((NpgsqlConnection)connection).OpenAsync();

            return await connection.GetAsync<T>(id, transaction: _transaction);
        }
        finally
        {
            if (shouldDispose)
                connection.Dispose();
        }
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        var connection = CreateConnection();
        var shouldDispose = _sharedConnection == null;

        try
        {
            if (shouldDispose)
                await ((NpgsqlConnection)connection).OpenAsync();

            var id = await connection.InsertAsync(entity, transaction: _transaction);

            // Se a entidade tem propriedade Id, define o valor retornado
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty != null && idProperty.CanWrite)
            {
                idProperty.SetValue(entity, Convert.ChangeType(id, idProperty.PropertyType));
            }

            return entity;
        }
        finally
        {
            if (shouldDispose)
                connection.Dispose();
        }
    }

    public virtual async Task<bool> UpdateAsync(T entity)
    {
        var connection = CreateConnection();
        var shouldDispose = _sharedConnection == null;

        try
        {
            if (shouldDispose)
                await ((NpgsqlConnection)connection).OpenAsync();

            return await connection.UpdateAsync(entity, transaction: _transaction);
        }
        finally
        {
            if (shouldDispose)
                connection.Dispose();
        }
    }
    public virtual async Task<bool> DeleteAsync(int id)
    {
        var connection = CreateConnection();
        var shouldDispose = _sharedConnection == null;

        try
        {
            if (shouldDispose)
                await ((NpgsqlConnection)connection).OpenAsync();

            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            return await connection.DeleteAsync(entity, transaction: _transaction);
        }
        finally
        {
            if (shouldDispose)
                connection.Dispose();
        }
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        var connection = CreateConnection();
        var shouldDispose = _sharedConnection == null;

        try
        {
            if (shouldDispose)
                await ((NpgsqlConnection)connection).OpenAsync();

            var entity = await connection.GetAsync<T>(id, transaction: _transaction);
            return entity != null;
        }
        finally
        {
            if (shouldDispose)
                connection.Dispose();
        }
    }

    public virtual async Task<int> CountAsync()
    {
        var connection = CreateConnection();
        var shouldDispose = _sharedConnection == null;

        try
        {
            if (shouldDispose)
                await ((NpgsqlConnection)connection).OpenAsync();

            var tableName = GetTableName();
            var sql = $"SELECT COUNT(*) FROM {tableName}";
            return await connection.QuerySingleAsync<int>(sql, transaction: _transaction);
        }
        finally
        {
            if (shouldDispose)
                connection.Dispose();
        }
    }

    protected virtual string GetTableName()
    {
        // Por padrão, usa o nome da classe como nome da tabela
        // Pode ser sobrescrito por classes filhas se necessário
        return typeof(T).Name + "s";
    }

    // Implementação dos métodos em português para compatibilidade
    public virtual async Task<IEnumerable<T>> GetTodosAsync()
    {
        return await GetAllAsync();
    }

    public virtual async Task<T?> GetPeloIdAsync(int id)
    {
        return await GetByIdAsync(id);
    }

    public virtual async Task<T> AdicionarAsync(T entity)
    {
        return await AddAsync(entity);
    }

    public virtual void Atualizar(T entity)
    {
        // Marca a entidade para atualização, será persistida no CommitAsync
        UpdateAsync(entity).Wait();
    }

    public virtual void Deletar(T entity)
    {
        // Extrai o Id da entidade para deletar
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty != null)
        {
            var id = (int)idProperty.GetValue(entity)!;
            DeleteAsync(id).Wait();
        }
    }
}