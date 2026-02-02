using FluentAssertions;
using Vendas.Domain.Catalogo.Entities;
using Vendas.Domain.Catalogo.Enums;
using Vendas.Domain.Catalogo.Events;
using Vendas.Domain.Catalogo.ValueObjects;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Domain.Tests.Catalogo.Entities
{
    public class ProdutoTests
    {
        private  Produto CriarProduto(string nome = "Câmera Profissional", string codigo = "CAM-001", decimal preco = 2500m, int estoque = 10, string? descricao = null)
        {
            return new Produto(
                new NomeProduto(nome),
                new CodigoProduto(codigo),
                new PrecoProduto(preco),
                Guid.NewGuid(),
                estoque,
                descricao);
        }
        
        [Fact]
        public void CriarProduto_DeveNascerAtivo()
        {
            var produto = CriarProduto();

            produto.Status.Should().Be(StatusProduto.Ativo);
        }

        [Fact]
        public void CriarProduto_ComEstoqueNegativo_DeveLancarExcecao()
        {
            Action act = () => CriarProduto(estoque: -1);

            act.Should().Throw<DomainException>().WithMessage("Estoque inicial não pode ser negativo.");
        }

        [Fact]
        public void AlterarNome_DeveAtualizarNome()
        {
            var produto = CriarProduto();
            var novoNome = "Câmera Mirrorless";

            produto.AlterarNome(new NomeProduto(novoNome));

            produto.Nome.Valor.Should().Be(novoNome);
        }

        [Fact]
        public void AlterarPreco_DeveAlterarPrecoEGerarEvento()
        {
            var produto = CriarProduto();
            produto.ClearDomainEvents();

            var novoPreco = new PrecoProduto(3000m);

            produto.AlterarPreco(novoPreco);

            produto.Preco.Valor.Should().Be(3000m);

            produto.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<PrecoProdutoAlteradoEvent>();

            var evt = (PrecoProdutoAlteradoEvent)produto.DomainEvents.Single();
            evt.PrecoAntigo.Should().Be(2500m);
            evt.PrecoNovo.Should().Be(3000m);
        }

        [Fact]
        public void AlterarEstoque_DeveAlterarEstoqueEGerarEvento()
        {
            var produto = CriarProduto();
            produto.ClearDomainEvents();

            produto.AjustarEstoque(5, "Reposição");

            produto.Estoque.Should().Be(15);

            produto.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<EstoqueAjustadoEvent>();
        }

        [Fact]
        public void AjustarEstoque_ResultadoNegativo_DeveLancarExcecao()
        {
            var produto = CriarProduto(estoque: 5);

            Action act = () => produto.AjustarEstoque(-10, "Erro de ajuste");

            act.Should().Throw<DomainException>().WithMessage("O ajuste de estoque resultaria em valor negativo.");
        }

        [Fact]
        public void Inativar_DeveMudarStatusEGerarEvento()
        {
            var produto = CriarProduto();
            produto.ClearDomainEvents();

            produto.Inativar();

            produto.Status.Should().Be(StatusProduto.Inativo);

            produto.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<ProdutoInativadoEvent>();
        }

        [Fact]
        public void Ativar_DeveMudarStatusEGerarEvento()
        {
            var produto = CriarProduto();
            produto.Inativar();
            produto.ClearDomainEvents();

            produto.Ativar();

            produto.Status.Should().Be(StatusProduto.Ativo);
            produto.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<ProdutoAtivadoEvent>();
        }

        [Fact]
        public void Inativar_QuandoJaInativo_DeveLancarException()
        {
            var produto = CriarProduto();
            produto.Inativar();

            Action act = () => produto.Inativar();

            act.Should().Throw<DomainException>().WithMessage("O produto já está inativo.");
        }

        [Fact]
        public void Ativar_QuandoJaAtivo_DeveLancarException()
        {
            var produto = CriarProduto();

            Action act = () => produto.Ativar();

            act.Should().Throw<DomainException>().WithMessage("O produto já está ativo.");
        }

        [Fact]
        public void AlterarDescricao_DeveAtualizarDescricao()
        {
            var produto = CriarProduto();

            produto.AlterarDescricao("Descrição atualizada");

            produto.Descricao.Should().Be("Descrição atualizada");
        }

        [Fact]
        public void AdicionarImagem_DeveAdicionarImagemEGerarEvento()
        {
            var produto = CriarProduto();
            produto.ClearDomainEvents();

            var imagem = new ImagemProduto("http://img.com/1.jpg", 1);

            produto.AdicionarImagem(imagem);

            produto.Imagens.Should().HaveCount(1);

            produto.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<ImagemAdicionadaEvent>();
        }

        [Fact]
        public void AdicionarImagem_ComOrdemDuplicada_DeveLancarExcecao()
        {
            var produto = CriarProduto();
            produto.AdicionarImagem(new ImagemProduto("http://img.com/1.jpg", 1));

            Action act = () => produto.AdicionarImagem(new ImagemProduto("http://img.com/2.jpg", 1));

            act.Should().Throw<DomainException>().WithMessage("Já existe uma imagem com esta ordem.");
        }
    }
}
