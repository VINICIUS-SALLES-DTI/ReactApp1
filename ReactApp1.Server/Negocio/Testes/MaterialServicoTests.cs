using FluentAssertions;
using Moq;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios.Interfaces;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.UnitOfWorks.Interfaces;
using ReactApp1.Server.CrossCutting.DTOs;
using ReactApp1.Server.Negocio.Servicos;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace ReactApp1.Server.Tests.Servicos;

public class MaterialServicoTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMaterialRepository> _mockMaterialRepository;
    private readonly MaterialServico _materialServico;

    public MaterialServicoTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMaterialRepository = new Mock<IMaterialRepository>();

        _mockUnitOfWork.Setup(u => u.Materiais).Returns(_mockMaterialRepository.Object);

        _materialServico = new MaterialServico(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarListaDeMateriais()
    {
        // Arrange
        var materiais = new List<Material>
        {
            new Material { Id = 1, Nome = "Cimento", PrecoUnitario = 30.50m, Disponivel = true },
            new Material { Id = 2, Nome = "Areia", PrecoUnitario = 50.00m, Disponivel = true }
        };

        _mockMaterialRepository.Setup(r => r.GetTodosAsync())
            .ReturnsAsync(materiais);

        // Act
        var resultado = await _materialServico.ObterTodosAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.First().Nome.Should().Be("Cimento");
        resultado.Last().Nome.Should().Be("Areia");
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdValido_DeveRetornarMaterial()
    {
        // Arrange
        var material = new Material
        {
            Id = 1,
            Nome = "Cimento",
            PrecoUnitario = 30.50m,
            Disponivel = true
        };

        _mockMaterialRepository.Setup(r => r.GetPeloIdAsync(1))
            .ReturnsAsync(material);

        // Act
        var resultado = await _materialServico.ObterPorIdAsync(1);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(1);
        resultado.Nome.Should().Be("Cimento");
        resultado.PrecoUnitario.Should().Be(30.50m);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdInvalido_DeveRetornarNull()
    {
        // Arrange
        _mockMaterialRepository.Setup(r => r.GetPeloIdAsync(999))
            .ReturnsAsync((Material?)null);

        // Act
        var resultado = await _materialServico.ObterPorIdAsync(999);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task CriarAsync_ComDadosValidos_DeveCriarMaterial()
    {
        // Arrange
        var materialDto = new MaterialCriacaoDto
        {
            Nome = "Brita",
            PrecoUnitario = 80.00m
        };

        var materialCriado = new Material
        {
            Id = 1,
            Nome = materialDto.Nome,
            PrecoUnitario = materialDto.PrecoUnitario,
            Disponivel = true
        };

        _mockMaterialRepository.Setup(r => r.AdicionarAsync(It.IsAny<Material>()))
            .Callback<Material>(m => m.Id = 1)
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(u => u.CompletarAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _materialServico.CriarAsync(materialDto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Nome.Should().Be("Brita");
        resultado.PrecoUnitario.Should().Be(80.00m);
        resultado.Disponivel.Should().BeTrue();

        _mockMaterialRepository.Verify(r => r.AdicionarAsync(It.IsAny<Material>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompletarAsync(), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_ComDadosInvalidos_DeveLancarValidationException()
    {
        // Arrange: nome vazio e preco negativo -> falha nas DataAnnotations
        var materialDto = new MaterialCriacaoDto
        {
            Nome = "",
            PrecoUnitario = -5.0m
        };

        // Act
        Func<Task> act = async () => await _materialServico.CriarAsync(materialDto);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        _mockMaterialRepository.Verify(r => r.AdicionarAsync(It.IsAny<Material>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CompletarAsync(), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_ComDadosInvalidos_DeveLancarValidationException()
    {
        // Arrange: obter um material existente
        var materialExistente = new Material
        {
            Id = 1,
            Nome = "Cimento",
            PrecoUnitario = 30.50m,
            Disponivel = true
        };

        _mockMaterialRepository.Setup(r => r.GetPeloIdAsync(1))
            .ReturnsAsync(materialExistente);

        var materialDto = new MaterialCriacaoDto
        {
            Nome = "",
            PrecoUnitario = -1.0m
        };

        // Act
        Func<Task> act = async () => await _materialServico.AtualizarAsync(1, materialDto);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        _mockMaterialRepository.Verify(r => r.Atualizar(It.IsAny<Material>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CompletarAsync(), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_ComIdValido_DeveAtualizarMaterial()
    {
        // Arrange
        var materialExistente = new Material
        {
            Id = 1,
            Nome = "Cimento",
            PrecoUnitario = 30.50m,
            Disponivel = true
        };

        var materialDto = new MaterialCriacaoDto
        {
            Nome = "Cimento CP-II",
            PrecoUnitario = 35.00m
        };

        _mockMaterialRepository.Setup(r => r.GetPeloIdAsync(1))
            .ReturnsAsync(materialExistente);

        _mockUnitOfWork.Setup(u => u.CompletarAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _materialServico.AtualizarAsync(1, materialDto);

        // Assert
        resultado.Should().BeTrue();
        materialExistente.Nome.Should().Be("Cimento CP-II");
        materialExistente.PrecoUnitario.Should().Be(35.00m);

        _mockMaterialRepository.Verify(r => r.Atualizar(materialExistente), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompletarAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_ComIdInvalido_DeveRetornarFalse()
    {
        // Arrange
        var materialDto = new MaterialCriacaoDto
        {
            Nome = "Cimento",
            PrecoUnitario = 30.50m
        };

        _mockMaterialRepository.Setup(r => r.GetPeloIdAsync(999))
            .ReturnsAsync((Material?)null);

        // Act
        var resultado = await _materialServico.AtualizarAsync(999, materialDto);

        // Assert
        resultado.Should().BeFalse();
        _mockMaterialRepository.Verify(r => r.Atualizar(It.IsAny<Material>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CompletarAsync(), Times.Never);
    }

    [Fact]
    public async Task ExcluirAsync_ComIdValido_DeveExcluirMaterial()
    {
        // Arrange
        var material = new Material
        {
            Id = 1,
            Nome = "Cimento",
            PrecoUnitario = 30.50m,
            Disponivel = true
        };

        _mockMaterialRepository.Setup(r => r.GetPeloIdAsync(1))
            .ReturnsAsync(material);

        _mockUnitOfWork.Setup(u => u.CompletarAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _materialServico.ExcluirAsync(1);

        // Assert
        resultado.Should().BeTrue();
        _mockMaterialRepository.Verify(r => r.Deletar(material), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompletarAsync(), Times.Once);
    }

    [Fact]
    public async Task ExcluirAsync_ComIdInvalido_DeveRetornarFalse()
    {
        // Arrange
        _mockMaterialRepository.Setup(r => r.GetPeloIdAsync(999))
            .ReturnsAsync((Material?)null);

        // Act
        var resultado = await _materialServico.ExcluirAsync(999);

        // Assert
        resultado.Should().BeFalse();
        _mockMaterialRepository.Verify(r => r.Deletar(It.IsAny<Material>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CompletarAsync(), Times.Never);
    }

    [Fact]
    public async Task PesquisarAsync_ComTermoValido_DeveRetornarMateriaisFiltrados()
    {
        // Arrange
        var materiais = new List<Material>
        {
            new Material { Id = 1, Nome = "Cimento", PrecoUnitario = 30.50m, Disponivel = true },
            new Material { Id = 2, Nome = "Areia", PrecoUnitario = 50.00m, Disponivel = true },
            new Material { Id = 3, Nome = "Brita", PrecoUnitario = 80.00m, Disponivel = true }
        };

        _mockMaterialRepository.Setup(r => r.GetTodosAsync())
            .ReturnsAsync(materiais);

        // Act
        var resultado = await _materialServico.PesquisarAsync("ci");

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(1);
        resultado.First().Nome.Should().Be("Cimento");
    }

    [Fact]
    public async Task PesquisarAsync_ComTermoInvalido_DeveRetornarListaVazia()
    {
        // Arrange
        var materiais = new List<Material>
        {
            new Material { Id = 1, Nome = "Cimento", PrecoUnitario = 30.50m, Disponivel = true },
            new Material { Id = 2, Nome = "Areia", PrecoUnitario = 50.00m, Disponivel = true },
            new Material { Id = 3, Nome = "Brita", PrecoUnitario = 80.00m, Disponivel = true }
        };

        _mockMaterialRepository.Setup(r => r.GetTodosAsync())
            .ReturnsAsync(materiais);

        // Act
        var resultado = await _materialServico.PesquisarAsync("Asfalto");

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }
}
