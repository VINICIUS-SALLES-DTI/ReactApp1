
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.UnitOfWorks.Interfaces;
using ReactApp1.Server.CrossCutting.DTOs;
using System.ComponentModel.DataAnnotations;

namespace ReactApp1.Server.Negocio.Servicos;

public class TracoServico : ITracoServico
{
    private readonly IUnitOfWork _unitOfWork;
    // Se você usar AutoMapper, injete-o aqui também
    // private readonly IMapper _mapper;

    public TracoServico(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CustoTracoDto?> CalcularCustoPorMetroCubicoAsync(int tracoId)
    {
        var traco = await _unitOfWork.Tracos.GetTracoComComponentesAsync(tracoId);

        if (traco == null || !traco.TracoMateriais.Any())
        {
            return null; // ou lançar uma exceção
        }

        // A lógica de cálculo que já definimos
        decimal custoTotal = traco.TracoMateriais
                                  .Sum(tm => tm.Quantidade * tm.Material.PrecoUnitario);

        // Mapeamento para o DTO (pode ser manual ou com AutoMapper)
        var custoDto = new CustoTracoDto
        {
            TracoId = traco.Id,
            NomeTraco = traco.Nome,
            CustoTotalPorM3 = custoTotal
        };

        return custoDto;
    }

    public async Task<TracoDto> CriarAsync(TracoCriacaoDto tracoDto)
    {
        if (tracoDto == null)
            throw new ArgumentNullException(nameof(tracoDto));

        var traco = new Traco
        {
            Nome = tracoDto.Nome,
            ResistenciaFck = tracoDto.ResistenciaFck,
            Slump = tracoDto.Slump,
            TracoMateriais = tracoDto.Componentes?.Select(c => new TracoMaterial
            {
                MaterialId = c.MaterialId,
                Quantidade = c.Quantidade,
                UnidadeMedida = c.UnidadeMedida
            }).ToList() ?? new List<TracoMaterial>()
        };

        // Validar DataAnnotations na entidade Traco (nome, ranges etc.)
        Validator.ValidateObject(traco, new ValidationContext(traco), validateAllProperties: true);

        // Validações mais complexas que envolvem regras de negócio: componentes
        if (traco.TracoMateriais == null || !traco.TracoMateriais.Any())
            throw new ArgumentException("Pelo menos um componente deve ser informado.", nameof(tracoDto.Componentes));

        foreach (var c in traco.TracoMateriais)
        {
            if (c.MaterialId <= 0)
                throw new ArgumentException("MaterialId inválido em um componente.", nameof(tracoDto.Componentes));
            if (c.Quantidade <= 0)
                throw new ArgumentException("Quantidade de um componente deve ser maior que zero.", nameof(tracoDto.Componentes));
            if (string.IsNullOrWhiteSpace(c.UnidadeMedida))
                throw new ArgumentException("Unidade de medida é obrigatória em um componente.", nameof(tracoDto.Componentes));
        }
        // Adicionar o traço (o EF vai gerenciar os TracoMateriais automaticamente)
        // Adicionar o traço (o EF vai gerenciar os TracoMateriais automaticamente)
        await _unitOfWork.Tracos.AdicionarAsync(traco);
        await _unitOfWork.CompletarAsync();

        // Retornar o DTO
        return new TracoDto
        {
            Id = traco.Id,
            Nome = traco.Nome,
            ResistenciaFck = traco.ResistenciaFck,
            Slump = traco.Slump
        };
    }

    public async Task<TracoDto?> ObterPorIdAsync(int id)
    {
        var traco = await _unitOfWork.Tracos.GetPeloIdAsync(id);

        if (traco == null)
            return null;

        return new TracoDto
        {
            Id = traco.Id,
            Nome = traco.Nome,
            ResistenciaFck = traco.ResistenciaFck,
            Slump = traco.Slump
        };
    }

    public async Task<TracoDetalhesDto?> ObterDetalhesAsync(int id)
    {
        var traco = await _unitOfWork.Tracos.GetTracoComComponentesAsync(id);

        if (traco == null)
            return null;

        var componentes = traco.TracoMateriais.Select(tm => new ComponenteDto
        {
            MaterialId = tm.MaterialId,
            NomeMaterial = tm.Material.Nome,
            Quantidade = tm.Quantidade,
            UnidadeMedida = tm.UnidadeMedida
        }).ToList();

        return new TracoDetalhesDto
        {
            Id = traco.Id,
            Nome = traco.Nome,
            ResistenciaFck = traco.ResistenciaFck,
            Slump = traco.Slump,
            Componentes = componentes
        };
    }

    public async Task<IEnumerable<TracoDto>> ObterTodosAsync()
    {
        var tracos = await _unitOfWork.Tracos.GetTodosAsync();

        return tracos.Select(t => new TracoDto
        {
            Id = t.Id,
            Nome = t.Nome,
            ResistenciaFck = t.ResistenciaFck,
            Slump = t.Slump
        });
    }

    public async Task<TracoDto?> AtualizarAsync(int id, TracoCriacaoDto tracoDto)
    {
        var traco = await _unitOfWork.Tracos.GetTracoComComponentesAsync(id);

        if (traco == null)
            return null;


        // Validar entrada básica via DataAnnotations na própria entidade
        if (tracoDto == null)
            throw new ArgumentNullException(nameof(tracoDto));

        traco.Nome = tracoDto.Nome;
        traco.ResistenciaFck = tracoDto.ResistenciaFck;
        traco.Slump = tracoDto.Slump;

        Validator.ValidateObject(traco, new ValidationContext(traco), validateAllProperties: true);

        // Validações mais complexas: componentes

        // Limpar componentes antigos
        traco.TracoMateriais.Clear();

        // Adicionar novos componentes
        foreach (var componente in tracoDto.Componentes)
        {
            traco.TracoMateriais.Add(new TracoMaterial
            {
                TracoId = traco.Id,
                MaterialId = componente.MaterialId,
                Quantidade = componente.Quantidade,
                UnidadeMedida = componente.UnidadeMedida
            });
        }

        _unitOfWork.Tracos.Atualizar(traco);
        await _unitOfWork.CompletarAsync();

        return new TracoDto
        {
            Id = traco.Id,
            Nome = traco.Nome,
            ResistenciaFck = traco.ResistenciaFck,
            Slump = traco.Slump
        };
    }

    public async Task<bool> ExcluirAsync(int id)
    {
        var traco = await _unitOfWork.Tracos.GetPeloIdAsync(id);

        if (traco == null)
            return false;

        // O EF vai gerenciar automaticamente a exclusão dos TracoMateriais
        // devido à configuração de cascade delete ou podemos carregar com componentes
        _unitOfWork.Tracos.Deletar(traco);
        await _unitOfWork.CompletarAsync();

        return true;
    }

    public async Task<IEnumerable<TracoDto>> PesquisarAsync(string termo)
    {
        var tracos = await _unitOfWork.Tracos.GetTodosAsync();

        if (string.IsNullOrWhiteSpace(termo))
            return tracos.Select(t => new TracoDto
            {
                Id = t.Id,
                Nome = t.Nome,
                ResistenciaFck = t.ResistenciaFck,
                Slump = t.Slump
            });

        var termoLower = termo.ToLower();
        var tracosFiltrados = tracos.Where(t =>
            t.Nome.ToLower().Contains(termoLower) ||
            t.ResistenciaFck.ToString().Contains(termoLower) ||
            t.Slump.ToString().Contains(termoLower));

        return tracosFiltrados.Select(t => new TracoDto
        {
            Id = t.Id,
            Nome = t.Nome,
            ResistenciaFck = t.ResistenciaFck,
            Slump = t.Slump
        });
    }
}
