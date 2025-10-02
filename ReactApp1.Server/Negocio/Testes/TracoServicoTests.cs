using FluentAssertions;
using Moq;
using System.ComponentModel.DataAnnotations;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Entidades;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.Repositorios.Interfaces;
using ReactApp1.Server.Apresentacao.Dependencias.Persistencia.UnitOfWorks.Interfaces;
using ReactApp1.Server.CrossCutting.DTOs;
using ReactApp1.Server.Negocio.Servicos;
using Xunit;

namespace ReactApp1.Server.Tests.Servicos;

public class TracoServicoTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ITracoRepository> _mockTracoRepository;
    private readonly TracoServico _tracoServico;

    public TracoServicoTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTracoRepository = new Mock<ITracoRepository>();

        _mockUnitOfWork.Setup(u => u.Tracos).Returns(_mockTracoRepository.Object);

        _tracoServico = new TracoServico(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarListaDeTracos()
    {
        // Arrange
        var tracos = new List<Traco>
        {
            new Traco { Id = 1, Nome = "Concreto FCK 25", ResistenciaFck = 25, Slump = 120 },
            new Traco { Id = 2, Nome = "Concreto FCK 30", ResistenciaFck = 30, Slump = 100 }
        };

        _mockTracoRepository.Setup(r => r.GetTodosAsync())
            .ReturnsAsync(tracos);

        // Act
        var resultado = await _tracoServico.ObterTodosAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.First().Nome.Should().Be("Concreto FCK 25");
        resultado.Last().Nome.Should().Be("Concreto FCK 30");
    }

    [Fact]
    public async Task ObterDetalhesAsync_ComIdValido_DeveRetornarTracoComComponentes()
    {
        // Arrange
        var material1 = new Material { Id = 1, Nome = "Cimento", PrecoUnitario = 30.50m };
        var material2 = new Material { Id = 2, Nome = "Areia", PrecoUnitario = 50.00m };

        var traco = new Traco
        {
            Id = 1,
            Nome = "Concreto FCK 25",
            ResistenciaFck = 25,
            Slump = 120,
            TracoMateriais = new List<TracoMaterial>
            {
                new TracoMaterial
                {
                    MaterialId = 1,
                    Material = material1,
                    Quantidade = 300,
                    UnidadeMedida = "Kg"
                },
                new TracoMaterial
                {
                    MaterialId = 2,
                    Material = material2,
                    Quantidade = 800,
                    UnidadeMedida = "Kg"
                }
            }
        };

        _mockTracoRepository.Setup(r => r.GetTracoComComponentesAsync(1))
            .ReturnsAsync(traco);

        // Act
        var resultado = await _tracoServico.ObterDetalhesAsync(1);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Nome.Should().Be("Concreto FCK 25");
        resultado.Componentes.Should().HaveCount(2);
        resultado.Componentes.First().NomeMaterial.Should().Be("Cimento");
        resultado.Componentes.Last().NomeMaterial.Should().Be("Areia");
    }

    [Fact]
    public async Task ObterDetalhesAsync_ComIdInvalido_DeveRetornarNull()
    {
        // Arrange
        _mockTracoRepository.Setup(r => r.GetTracoComComponentesAsync(999))
            .ReturnsAsync((Traco?)null);

        // Act
        var resultado = await _tracoServico.ObterDetalhesAsync(999);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task CriarAsync_ComDadosValidos_DeveCriarTraco()
    {
        // Arrange
        var tracoDto = new TracoCriacaoDto
        {
            Nome = "Concreto FCK 25",
            ResistenciaFck = 25,
            Slump = 120,
            Componentes = new List<ComponenteCriacaoDto>
            {
                new ComponenteCriacaoDto
                {
                    MaterialId = 1,
                    Quantidade = 300,
                    UnidadeMedida = "Kg"
                },
                new ComponenteCriacaoDto
                {
                    MaterialId = 2,
                    Quantidade = 800,
                    UnidadeMedida = "Kg"
                }
            }
        };

        _mockTracoRepository.Setup(r => r.AdicionarAsync(It.IsAny<Traco>()))
            .Callback<Traco>(t => t.Id = 1)
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(u => u.CompletarAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _tracoServico.CriarAsync(tracoDto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Nome.Should().Be("Concreto FCK 25");
        resultado.ResistenciaFck.Should().Be(25);
        resultado.Slump.Should().Be(120);

        _mockTracoRepository.Verify(r => r.AdicionarAsync(It.Is<Traco>(
            t => t.TracoMateriais.Count == 2)), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompletarAsync(), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_ComDadosInvalidos_NaoDeveCriarTraco()
    {
        // Arrange: dados com falha nas DataAnnotations (nome vazio e resistencia negativa)
        var tracoDto = new TracoCriacaoDto
        {
            Nome = "", // Nome inválido
            ResistenciaFck = -10, // Resistencia inválida
            Slump = 120,
            Componentes = new List<ComponenteCriacaoDto>()
        };

        // Act & Assert: validação da entidade via DataAnnotations deve lançar ValidationException
        Func<Task> actEntity = async () => await _tracoServico.CriarAsync(tracoDto);
        await actEntity.Should().ThrowAsync<ValidationException>();

        // Arrange: dados com entidade válida mas sem componentes -> regra de negócio do serviço
        var tracoDto2 = new TracoCriacaoDto
        {
            Nome = "Concreto FCK",
            ResistenciaFck = 25,
            Slump = 120,
            Componentes = new List<ComponenteCriacaoDto>() // vazio
        };

        // Act & Assert: deve lançar ArgumentException por falta de componentes
        Func<Task> actBusiness = async () => await _tracoServico.CriarAsync(tracoDto2);
        await actBusiness.Should().ThrowAsync<ArgumentException>();

        _mockTracoRepository.Verify(r => r.AdicionarAsync(It.IsAny<Traco>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CompletarAsync(), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_ComIdValido_DeveAtualizarTraco()
    {
        // Arrange
        var tracoExistente = new Traco
        {
            Id = 1,
            Nome = "Concreto FCK 25",
            ResistenciaFck = 25,
            Slump = 120,
            TracoMateriais = new List<TracoMaterial>()
        };

        var tracoDto = new TracoCriacaoDto
        {
            Nome = "Concreto FCK 30",
            ResistenciaFck = 30,
            Slump = 100,
            Componentes = new List<ComponenteCriacaoDto>
            {
                new ComponenteCriacaoDto
                {
                    MaterialId = 1,
                    Quantidade = 350,
                    UnidadeMedida = "Kg"
                }
            }
        };

        _mockTracoRepository.Setup(r => r.GetTracoComComponentesAsync(1))
            .ReturnsAsync(tracoExistente);

        _mockUnitOfWork.Setup(u => u.CompletarAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _tracoServico.AtualizarAsync(1, tracoDto);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Nome.Should().Be("Concreto FCK 30");
        resultado.ResistenciaFck.Should().Be(30);
        resultado.Slump.Should().Be(100);

        tracoExistente.Nome.Should().Be("Concreto FCK 30");
        tracoExistente.TracoMateriais.Should().HaveCount(1);

        _mockTracoRepository.Verify(r => r.Atualizar(tracoExistente), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompletarAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_ComIdInvalido_DeveRetornarNull()
    {
        // Arrange
        var tracoDto = new TracoCriacaoDto
        {
            Nome = "Concreto FCK 25",
            ResistenciaFck = 25,
            Slump = 120,
            Componentes = new List<ComponenteCriacaoDto>()
        };

        _mockTracoRepository.Setup(r => r.GetTracoComComponentesAsync(999))
            .ReturnsAsync((Traco?)null);

        // Act
        var resultado = await _tracoServico.AtualizarAsync(999, tracoDto);

        // Assert
        resultado.Should().BeNull();
        _mockTracoRepository.Verify(r => r.Atualizar(It.IsAny<Traco>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CompletarAsync(), Times.Never);
    }

    [Fact]
    public async Task ExcluirAsync_ComIdValido_DeveExcluirTraco()
    {
        // Arrange
        var traco = new Traco
        {
            Id = 1,
            Nome = "Concreto FCK 25",
            ResistenciaFck = 25,
            Slump = 120
        };

        _mockTracoRepository.Setup(r => r.GetPeloIdAsync(1))
            .ReturnsAsync(traco);

        _mockUnitOfWork.Setup(u => u.CompletarAsync())
            .ReturnsAsync(1);

        // Act
        var resultado = await _tracoServico.ExcluirAsync(1);

        // Assert
        resultado.Should().BeTrue();
        _mockTracoRepository.Verify(r => r.Deletar(traco), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompletarAsync(), Times.Once);
    }

    [Fact]
    public async Task ExcluirAsync_ComIdInvalido_DeveRetornarFalse()
    {
        // Arrange
        _mockTracoRepository.Setup(r => r.GetPeloIdAsync(999))
            .ReturnsAsync((Traco?)null);

        // Act
        var resultado = await _tracoServico.ExcluirAsync(999);

        // Assert
        resultado.Should().BeFalse();
        _mockTracoRepository.Verify(r => r.Deletar(It.IsAny<Traco>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.CompletarAsync(), Times.Never);
    }

    [Fact]
    public async Task CalcularCustoPorMetroCubicoAsync_ComTracoValido_DeveCalcularCusto()
    {
        // Arrange
        var material1 = new Material { Id = 1, Nome = "Cimento", PrecoUnitario = 30.50m };
        var material2 = new Material { Id = 2, Nome = "Areia", PrecoUnitario = 50.00m };

        var traco = new Traco
        {
            Id = 1,
            Nome = "Concreto FCK 25",
            ResistenciaFck = 25,
            Slump = 120,
            TracoMateriais = new List<TracoMaterial>
            {
                new TracoMaterial
                {
                    MaterialId = 1,
                    Material = material1,
                    Quantidade = 10,
                    UnidadeMedida = "Kg"
                },
                new TracoMaterial
                {
                    MaterialId = 2,
                    Material = material2,
                    Quantidade = 20,
                    UnidadeMedida = "Kg"
                }
            }
        };

        _mockTracoRepository.Setup(r => r.GetTracoComComponentesAsync(1))
            .ReturnsAsync(traco);

        // Act
        var resultado = await _tracoServico.CalcularCustoPorMetroCubicoAsync(1);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.TracoId.Should().Be(1);
        resultado.NomeTraco.Should().Be("Concreto FCK 25");
        // Custo = (10 * 30.50) + (20 * 50.00) = 305 + 1000 = 1305
        resultado.CustoTotalPorM3.Should().Be(1305.00m);
    }

    [Fact]
    public async Task CalcularCustoPorMetroCubicoAsync_ComTracoInvalido_DeveRetornarNull()
    {
        // Arrange
        _mockTracoRepository.Setup(r => r.GetTracoComComponentesAsync(999))
            .ReturnsAsync((Traco?)null);

        // Act
        var resultado = await _tracoServico.CalcularCustoPorMetroCubicoAsync(999);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task CalcularCustoPorMetroCubicoAsync_ComTracoSemComponentes_DeveRetornarNull()
    {
        // Arrange
        var traco = new Traco
        {
            Id = 1,
            Nome = "Concreto FCK 25",
            ResistenciaFck = 25,
            Slump = 120,
            TracoMateriais = new List<TracoMaterial>()
        };

        _mockTracoRepository.Setup(r => r.GetTracoComComponentesAsync(1))
            .ReturnsAsync(traco);

        // Act
        var resultado = await _tracoServico.CalcularCustoPorMetroCubicoAsync(1);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task PesquisarAsync_ComTermoValido_DeveRetornarTracosFiltrados()
    {
        // Arrange
        var tracos = new List<Traco>
        {
            new Traco { Id = 1, Nome = "Concreto FCK 25", ResistenciaFck = 25, Slump = 120 },
            new Traco { Id = 2, Nome = "Concreto FCK 30", ResistenciaFck = 30, Slump = 100 },
            new Traco { Id = 3, Nome = "Argamassa", ResistenciaFck = 15, Slump = 80 }
        };
        _mockTracoRepository.Setup(r => r.GetTodosAsync())
            .ReturnsAsync(tracos);

        // Act
        var resultado = await _tracoServico.PesquisarAsync("Concreto");

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.First().Nome.Should().Be("Concreto FCK 25");
        resultado.Last().Nome.Should().Be("Concreto FCK 30");
    }

    [Fact]
    public async Task PesquisarAsync_ComTermoInvalido_DeveRetornarListaVazia()
    {
        // Arrange
        var tracos = new List<Traco>
        {
            new Traco { Id = 1, Nome = "Concreto FCK 25", ResistenciaFck = 25, Slump = 120 },
        };
        
        _mockTracoRepository.Setup(r => r.GetTodosAsync())
            .ReturnsAsync(tracos);

        // Act
        var resultado = await _tracoServico.PesquisarAsync("Asfalto");

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }
}
