using FluentAssertions;
using Vendas.Domain.Common.Base;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Pedidos.Entities;

namespace Vendas.Domain.Tests.Pedidos.Entities
{
    public class ItemPedidoTests
    {
        private static ItemPedido CriarItemValido(decimal preco = 100m, int quantidade = 2)
        {
            return new ItemPedido(Guid.NewGuid(), "Produto Teste", preco, quantidade);
        }

        [Fact(DisplayName = "Deve criar ItemPedido com sucesso quando dados válidos")]
        public void Criar_DeveRetornarItemPedido_QuandoDadosValidos()
        {
            // Arrange
            var produtoId = Guid.NewGuid();
            var nomeProduto = "Produto Teste";
            var preco = 150m;
            var quantidade = 3;

            // Act
            var item = new ItemPedido(produtoId, nomeProduto, preco, quantidade);

            // Assert
            item.ProdutoId.Should().Be(produtoId);
            item.NomeProduto.Should().Be(nomeProduto);
            item.PrecoUnitario.Should().Be(preco);
            item.Quantidade.Should().Be(quantidade);
            item.DescontoAplicado.Should().Be(0);
            item.ValorTotal.Should().Be(preco * quantidade);
        }

        [Theory(DisplayName = "Deve lançar exceção ao criar ItemPedido com dados inválidos")]
        [InlineData("", "Produto A", 10, 1, "ProdutoId inválido.")]
        [InlineData("guid", "", 10, 1, "O nome do produto é obrigatório.")]
        [InlineData("guid", "Produto A", -5, 1, "O preço unitário deve ser maior que zero.")]
        [InlineData("guid", "Produto A", 10, 0, "A quantidade deve ser maior que zero.")]

        public void Criar_DeveLancarExcecao_QuandoDadosInvalidos(string tipo, string nomeProduto, decimal preco, int quantidade, string mensagemEsperada)
        {
            // Arrange
            Guid produtoId = tipo == "guid" ? Guid.NewGuid() : Guid.Empty;

            // Act
            Action act = () => new ItemPedido(produtoId, nomeProduto, preco, quantidade);

            // Assert
            act.Should().Throw<DomainException>().WithMessage(mensagemEsperada);
        }

        [Fact(DisplayName = "Deve aplicar desconto com sucesso quando valor válido")]
        public void AplicarDesconto_DeveAplicarComSucesso_QuandoValorValido()
        {
            // Arrange
            var item = CriarItemValido(preco: 200m, quantidade: 2);
            var desconto = 50m;

            // Act
            item.AplicarDesconto(desconto);

            // Assert
            item.DescontoAplicado.Should().Be(desconto);
            item.ValorTotal.Should().Be((item.PrecoUnitario * item.Quantidade) - desconto);
            item.DataAtualizacao.Should().NotBeNull();
        }

        [Theory(DisplayName = "Deve lançar exceção ao aplicar desconto inválido")]
        [InlineData(-10, "O desconto não pode ser negativo.")]
        [InlineData(500, "O desconto não pode ser maior que o valor total do item.")]

        public void AplicarDesconto_DeveLancarExcecao_QuandoValorInvalido(decimal desconto, string mensagemEsperada)
        {
            // Arrange
            var item = CriarItemValido(preco: 100m, quantidade: 2);

            // Act
            Action act = () => item.AplicarDesconto(desconto);

            // Assert
            act.Should().Throw<DomainException>().WithMessage(mensagemEsperada);
        }

        [Fact(DisplayName = "Deve adicionar unidades com sucesso quando valor válido")]
        public void AdicionarUnidades_DeveAdicionarComSucesso_QuandoValorValido()
        {
            // Arrange
            var item = CriarItemValido(preco: 100m, quantidade: 2);

            // Act
            item.AdicionarUnidades(3);

            // Assert
            item.Quantidade.Should().Be(5);
            item.ValorTotal.Should().Be(item.PrecoUnitario * item.Quantidade);
            item.DataAtualizacao.Should().NotBeNull();
        }

        [Fact(DisplayName = "Deve lançar exceção ao adicionar unidades inválidas")]
        public void AdicionarUnidades_DeveLancarExcecao_QuandoValorInvalido()
        {
            // Arrange
            var item = CriarItemValido(preco: 100m, quantidade: 2);
            // Act
            Action act = () => item.AdicionarUnidades(0);
            // Assert
            act.Should().Throw<DomainException>().WithMessage("Deve-se adicionar pelo menos uma unidade.");
        }

        [Fact(DisplayName = "Deve remover unidades com sucesso quando valor válido")]
        public void RemoverUnidades_DeveRemoverComSucesso_QuandoValorValido()
        {
            // Arrange
            var item = CriarItemValido(preco: 100m, quantidade: 5);

            // Act
            item.RemoverUnidades(2);

            // Assert
            item.Quantidade.Should().Be(3);
            item.ValorTotal.Should().Be(item.PrecoUnitario * item.Quantidade);
            item.DataAtualizacao.Should().NotBeNull();
        }

        [Theory(DisplayName = "Deve lançar exceção ao remover unidades inválidas")]
        [InlineData(0, "Deve-se remover pelo menos uma unidade.")]
        [InlineData(3, "Não é possível remover mais unidades do que o total existente.")]
        public void RemoverUnidades_DeveLancarExcecao_QuandoValorInvalido(int unidades, string mensagem)
        {
            // Arrange
            var item = CriarItemValido(preco: 100m, quantidade: 2);

            // Act
            Action act = () => item.RemoverUnidades(unidades);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage(mensagem);
        }

        [Fact(DisplayName = "Deve lançar exceção ao remover todas as unidades")]
        public void RemoverUnidades_DeveLancarExcecao_QuandoRemoverTodasUnidades()
        {
            // Arrange
            var item = CriarItemValido(preco: 100m, quantidade: 2);
            // Act
            Action act = () => item.RemoverUnidades(2);
            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("O item do pedido deve conter pelo menos uma unidade.");
        }

        [Fact(DisplayName = "Deve atualizar preço unitário com sucesso quando valor válido")]
        public void AtualizarPrecoUnitario_DeveAtualizarComSucesso_QuandoValorValido()
        {
            // Arrange
            var item = CriarItemValido(preco: 100m, quantidade: 2);

            // Act
            item.AtualizarPrecoUnitario(150m);

            // Assert
            item.PrecoUnitario.Should().Be(150m);
            item.ValorTotal.Should().Be(item.PrecoUnitario * item.Quantidade);
            item.DataAtualizacao.Should().NotBeNull();
        }

        [Fact(DisplayName = "Deve lançar exceção ao atualizar preço unitário inválido")]
        public void AtualizarPrecoUnitario_DeveLancarExcecao_QuandoValorInvalido()
        {
            // Arrange
            var item = CriarItemValido(preco: 100m, quantidade: 2);

            // Act
            Action act = () => item.AtualizarPrecoUnitario(0);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("O preço unitário deve ser maior que zero.");
        }

        [Fact(DisplayName = "Dois itens com mesmo Id devem ser considerados iguais")]
        public void Equals_DeveRetornarTrue_QuandoMesmoId()
        {
            // Arrange
            var item1 = CriarItemValido();
            var item2 = CriarItemValido();

            // Forçando mesmo Id para teste
            typeof(Entity).GetProperty("Id")!.SetValue(item2, item1.Id);

            // Act & Assert
            (item1 == item2).Should().BeTrue();
            item1.Equals(item2).Should().BeTrue();
        }
    }
}
