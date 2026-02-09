using FluentAssertions;
using Vendas.Domain.Catalogo;
using Vendas.Domain.Catalogo.Events;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Domain.Tests.Catalogo
{
    public class CategoriaTests
    {
        [Fact]
        public void CriarCategoria_DeveCriarAtivaComNomeValido()
        {
            var nome = "Eletrônicos";

            var categoria = new Categoria(nome);

            categoria.Nome.Should().Be(nome);
            categoria.Ativa.Should().BeTrue();
            categoria.DataCriacao.Should().NotBe(default);
            categoria.Descricao.Should().BeNull();
            categoria.DomainEvents.Should().BeEmpty();
        }

        [Fact]
        public void CriarCategoria_ComNomeInvalido_DeveLancarDomainException()
        {
            Action act = () => new Categoria("ab");

            act.Should().Throw<DomainException>().WithMessage("Nome deve ter ao menos 3 caracteres.");
        }

        [Fact]
        public void CriarCategoria_ComNomeVazio_DeveLancarDomainException()
        {
            Action act = () => new Categoria("");

            act.Should().Throw<DomainException>().WithMessage("Nome é obrigatório.");
        }

        [Fact]
        public void AlterarNome_DeveAtualizarNomeEDataAtualizacao()
        {
            var categoria = new Categoria("Acessórios");

            categoria.AlterarNome("Periféricos");

            categoria.Nome.Should().Be("Periféricos");
            categoria.DataAtualizacao.Should().NotBeNull();
        }

        [Fact]
        public void AlterarNome_ComNomeInvalido_DeveLancarDomainException()
        {
            var categoria = new Categoria("Acessórios");

            Action act = () => categoria.AlterarNome("ab");

            act.Should().Throw<DomainException>().WithMessage("Nome deve ter ao menos 3 caracteres.");
        }

        [Fact]
        public void AlterarDescricao_DeveAtualizarDescricaEDataAtualizacao()
        {
            var categoria = new Categoria("Acessórios", "Teste");

            categoria.AlterarDescricao("Teste 2");

            categoria.Descricao.Should().Be("Teste 2");
            categoria.DataAtualizacao.Should().NotBeNull();
        }

        [Fact]
        public void Ativar_DeveGerarEventoCategoriaAtivada()
        {
            var categoria = new Categoria("Jogos");
            categoria.Inativar();
            categoria.ClearDomainEvents();

            categoria.Ativar();
            var events = categoria.DomainEvents;

            events.Should().ContainSingle().Which.Should().BeOfType<CategoriaAtivadaEvent>();

            categoria.Ativa.Should().BeTrue();
        }

        [Fact]
        public void Ativar_QuandoJaAtiva_DeveLancarDomainException()
        {
            var categoria = new Categoria("Jogos");

            Action act = () => categoria.Ativar();

            act.Should().Throw<DomainException>().WithMessage("Categoria já está ativa.");
        }

        [Fact]
        public void Inativar_DeveGerarEventoCategoriaInativada()
        {
            var categoria = new Categoria("Jogos");
            categoria.Inativar();

            var events = categoria.DomainEvents;

            events.Should().ContainSingle().Which.Should().BeOfType<CategoriaInativadaEvent>();

            categoria.Ativa.Should().BeFalse();
        }

        [Fact]
        public void Inativar_QuandoJaInativa_DeveLancarDomainException()
        {
            var categoria = new Categoria("Jogos");
            categoria.Inativar();

            Action act = () => categoria.Inativar();

            act.Should().Throw<DomainException>().WithMessage("Categoria já está inativa.");
        }

        [Fact]
        public void DomainEvents_DeveSerPossivelLimparEventos()
        {
            var categoria = new Categoria("Jogos");
            categoria.Inativar();

            categoria.DomainEvents.Should().HaveCount(1);

            categoria.ClearDomainEvents();

            categoria.DomainEvents.Should().BeEmpty();
        }
    }
}
