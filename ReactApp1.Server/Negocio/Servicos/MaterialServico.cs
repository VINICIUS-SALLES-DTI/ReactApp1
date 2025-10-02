using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.UnitOfWorks.Interfaces;
using ReactApp1.Server.CrossCutting.DTOs;
using ReactApp1.Server.Negocio.Servicos.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ReactApp1.Server.Negocio.Servicos;

public class MaterialServico : IMaterialServico
{
    private readonly IUnitOfWork _unitOfWork;

    public MaterialServico(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<MaterialDto>> ObterTodosAsync()
    {
        var materiais = await _unitOfWork.Materiais.GetTodosAsync();

        return materiais.Select(m => new MaterialDto
        {
            Id = m.Id,
            Nome = m.Nome,
            PrecoUnitario = m.PrecoUnitario,
            Disponivel = m.Disponivel
        });
    }

    public async Task<MaterialDto?> ObterPorIdAsync(int id)
    {
        var material = await _unitOfWork.Materiais.GetPeloIdAsync(id);

        if (material == null)
            return null;

        return new MaterialDto
        {
            Id = material.Id,
            Nome = material.Nome,
            PrecoUnitario = material.PrecoUnitario,
            Disponivel = material.Disponivel
        };
    }

    public async Task<MaterialDto> CriarAsync(MaterialCriacaoDto materialDto)
    {
        if (materialDto == null)
            throw new ArgumentNullException(nameof(materialDto));

        var material = new Material
        {
            Nome = materialDto.Nome,
            PrecoUnitario = materialDto.PrecoUnitario,
            Disponivel = true // Por padrão, novos materiais estão disponíveis
        };

        // Validar DataAnnotations na entidade
        Validator.ValidateObject(material, new ValidationContext(material), validateAllProperties: true);

        await _unitOfWork.Materiais.AdicionarAsync(material);
        await _unitOfWork.CompletarAsync();

        return new MaterialDto
        {
            Id = material.Id,
            Nome = material.Nome,
            PrecoUnitario = material.PrecoUnitario,
            Disponivel = material.Disponivel
        };
    }

    public async Task<bool> AtualizarAsync(int id, MaterialCriacaoDto materialDto)
    {
        var material = await _unitOfWork.Materiais.GetPeloIdAsync(id);

        if (material == null)
            return false;

        if (materialDto == null)
            throw new ArgumentNullException(nameof(materialDto));

        material.Nome = materialDto.Nome;
        material.PrecoUnitario = materialDto.PrecoUnitario;

        // Validar DataAnnotations na entidade atualizada
        Validator.ValidateObject(material, new ValidationContext(material), validateAllProperties: true);

        _unitOfWork.Materiais.Atualizar(material);
        await _unitOfWork.CompletarAsync();

        return true;
    }

    public async Task<bool> ExcluirAsync(int id)
    {
        var material = await _unitOfWork.Materiais.GetPeloIdAsync(id);

        if (material == null)
            return false;

        // Opcionalmente, verificar se o material está sendo usado em algum traço
        // antes de permitir a exclusão
        _unitOfWork.Materiais.Deletar(material);
        await _unitOfWork.CompletarAsync();

        return true;
    }

    public async Task<IEnumerable<MaterialDto>> PesquisarAsync(string termo)
    {
        var materiais = await _unitOfWork.Materiais.GetTodosAsync();

        if (string.IsNullOrWhiteSpace(termo))
            return materiais.Select(m => new MaterialDto
            {
                Id = m.Id,
                Nome = m.Nome,
                PrecoUnitario = m.PrecoUnitario,
                Disponivel = m.Disponivel
            });

        var termoLower = termo.ToLower();
        var materiaisFiltrados = materiais.Where(m =>
            m.Nome.ToLower().Contains(termoLower) ||
            m.PrecoUnitario.ToString().Contains(termoLower));

        return materiaisFiltrados.Select(m => new MaterialDto
        {
            Id = m.Id,
            Nome = m.Nome,
            PrecoUnitario = m.PrecoUnitario,
            Disponivel = m.Disponivel
        });
    }
}
