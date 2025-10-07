using System.Data;
using Npgsql;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios.Interfaces;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.UnitOfWorks.Interfaces;

namespace ReactApp1.Server.Apresentacao.Dependencias.Persistencia.UnitOfWorks;

public class UnitOfWork : IUnitOfWork
{
    private readonly string _connectionString;
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;
    private bool _disposed = false;

    private IMaterialRepository? _materialRepository;
    private ITracoRepository? _tracoRepository;
    private ITracoMaterialRepository? _tracoMaterialRepository;

    public UnitOfWork(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IMaterialRepository Materiais
    {
        get
        {
            if (_materialRepository == null)
            {
                _materialRepository = new MaterialRepository(_connectionString);
            }
            return _materialRepository;
        }
    }

    public ITracoRepository Tracos
    {
        get
        {
            if (_tracoRepository == null)
            {
                _tracoRepository = new TracoRepository(_connectionString);
            }
            return _tracoRepository;
        }
    }

    public ITracoMaterialRepository TracoMateriais
    {
        get
        {
            if (_tracoMaterialRepository == null)
            {
                _tracoMaterialRepository = new TracoMaterialRepository(_connectionString);
            }
            return _tracoMaterialRepository;
        }
    }

    public async Task<int> CompletarAsync()
    {
        if (_transaction != null)
        {
            await CommitTransactionAsync();
            return 1;
        }
        return 0;
    }

    public async Task BeginTransactionAsync()
    {
        if (_connection == null)
        {
            _connection = new NpgsqlConnection(_connectionString);
            await _connection.OpenAsync();
        }

        if (_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync();
        }

        _transaction = await _connection.BeginTransactionAsync();

        // Configurar transação nos repositórios
        if (_materialRepository != null)
            _materialRepository.SetTransaction(_connection, _transaction);
        if (_tracoRepository != null)
            _tracoRepository.SetTransaction(_connection, _transaction);
        if (_tracoMaterialRepository != null)
            _tracoMaterialRepository.SetTransaction(_connection, _transaction);
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction?.Dispose();
            _connection?.Dispose();
            _disposed = true;
        }
    }
}
