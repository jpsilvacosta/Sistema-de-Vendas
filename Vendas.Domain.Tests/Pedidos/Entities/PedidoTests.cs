using FluentAssertions;
using System.Reflection;
using Vendas.Domain.Common.Enums;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Pedidos.Entities;
using Vendas.Domain.Pedidos.Events;
using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Domain.Tests.Pedidos.Entities
{
    public class PedidoTests
    {
        private static EnderecoEntrega CriarEnderecoValido()
            => EnderecoEntrega.Criar(
                cep: "12345-678",
                logradouro: "Rua Exemplo, 123",
                complemento: "Apto 45",
                bairro: "Bairro Exemplo",
                estado: "SP",
                cidade: "São Paulo",
                pais: "Brasil"
            );

        private static readonly Guid ClienteIdValido = Guid.NewGuid();
        private static readonly Guid ProdutoIdValido = Guid.NewGuid();

        private static void SetStatusPedido(Pedido pedido, StatusPedido status)
        {
            typeof(Pedido).GetProperty(nameof(Pedido.StatusPedido), BindingFlags.Public | BindingFlags.Instance)!.SetValue(pedido, status);
        }

        [Fact(DisplayName = "Deve criar pedido válido com status Pendente")]
        public void Deve_Criar_Pedido_Valido()
        {
            //Act
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());

            //Assert
            pedido.Should().NotBeNull();
            pedido.ClienteId.Should().Be(ClienteIdValido);
            pedido.EnderecoEntrega.Should().NotBeNull();
            pedido.StatusPedido.Should().Be(StatusPedido.Pendente);
            pedido.ValorTotal.Should().Be(0);
            pedido.Itens.Should().BeEmpty();
            pedido.Pagamentos.Should().BeEmpty();
            pedido.Id.Should().NotBeEmpty();
        }

        [Fact(DisplayName = "Não deve criar pedido com ClienteId inválido")]
        public void Nao_Deve_Criar_Pedido_Com_ClienteId_Invalido()
        {
            //Act
            Action act = () => Pedido.Criar(Guid.Empty, CriarEnderecoValido());

            //Assert
            act.Should().Throw<DomainException>()
                .WithMessage("ClienteId inválido.");

        }

        [Fact(DisplayName = "Não deve criar pedido sem endereço de entrega")]
        public void Nao_Deve_Criar_Pedido_Sem_Endereco_Entrega()
        {
            //Act
            Action act = () => Pedido.Criar(ClienteIdValido, null!);

            //Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Endereço de entrega obrigatório.");
        }

        [Fact(DisplayName = "Deve adicionar item ao pedido e recalcular valor total")]
        public void Deve_Adicionar_Item_Ao_Pedido()
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());

            //Act
            pedido.AdicionarItem(ProdutoIdValido, "Produto Exemplo", 50m, 2);

            //Assert
            pedido.Itens.Should().HaveCount(1);
            pedido.ValorTotal.Should().Be(100m);
            pedido.Itens.First().ValorTotal.Should().Be(100m);
        }

        [Fact(DisplayName = "Deve somar quantidade de item existente ao adicionar mesmo produto.")]
        public void Deve_Somar_Quantidade_Item_Existente()
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            var produtoId = ProdutoIdValido;

            //Act
            pedido.AdicionarItem(produtoId, "Produto Exemplo", 30m, 1);
            pedido.AdicionarItem(produtoId, "Produto Exemplo", 30m, 2);

            //Assert
            pedido.Itens.Should().HaveCount(1);
            var item = pedido.Itens.First();
            item.Quantidade.Should().Be(3);
            item.ValorTotal.Should().Be(90m);
            pedido.ValorTotal.Should().Be(90m);
        }

        [Theory(DisplayName = "Não deve permitir adicionar itens quando pedido não estiver Pendente")]
        [InlineData(StatusPedido.PagamentoConfirmado)]
        [InlineData(StatusPedido.EmSeparacao)]
        [InlineData(StatusPedido.Enviado)]
        [InlineData(StatusPedido.Entregue)]
        [InlineData(StatusPedido.Cancelado)]
        public void Nao_Deve_Adicionar_Item_Se_Pedido_Nao_Estiver_Pendente(StatusPedido status)
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            SetStatusPedido(pedido, status);

            //Act
            Action act = () => pedido.AdicionarItem(ProdutoIdValido, "Produto Exemplo", 50m, 1);

            //Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Itens só podem ser adicionados enquanto o pedido está pendente.");
        }

        [Fact(DisplayName = "Deve remover item e recalcular valor total")]
        public void Deve_Remover_Item_E_Recalcular_Valor_Total()
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Produto Exemplo", 40m, 3);

            //Act
            Action act = () => pedido.RemoverItem(pedido.Itens.First().Id);

            //Assert
            act.Should().Throw<DomainException>()
                .WithMessage("O pedido deve conter pelo menos um item.");
        }

        [Fact(DisplayName = "Deve remoter item e recalcular valor total quando houver mais de um item")]
        public void Deve_Remover_Item_E_Recalcular_Valor_Total_Quando_Houver_Mais_De_Um_Item()
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            var produto1 = Guid.NewGuid();
            var produto2 = Guid.NewGuid();

            //Act
            pedido.AdicionarItem(produto1, "Produto 1", 20m, 1);
            pedido.AdicionarItem(produto2, "Produto 2", 30m, 1);

            var itemId = pedido.Itens.First(i => i.ProdutoId == produto1).Id;
            pedido.RemoverItem(itemId);

            //Assert
            pedido.Itens.Should().HaveCount(1);
            pedido.ValorTotal.Should().Be(30m);
        }

        [Fact(DisplayName = "Deve ignorar remoção de item inexistente")]
        public void Deve_Ignorar_Remocao_De_Item_Inexistente()
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Produto Exemplo", 40m, 2);

            //Act
            Action act = () => pedido.RemoverItem(Guid.NewGuid());

            //Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Item não encontrado no pedido.");
        }

        [Theory(DisplayName = "Não deve permitir remover itens quando pedido não estiver Pendente")]
        [InlineData(StatusPedido.PagamentoConfirmado)]
        [InlineData(StatusPedido.EmSeparacao)]
        [InlineData(StatusPedido.Enviado)]
        [InlineData(StatusPedido.Entregue)]
        [InlineData(StatusPedido.Cancelado)]
        public void Nao_Deve_Remover_Item_Se_Pedido_Nao_Estiver_Pendente(StatusPedido status)
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Produto Exemplo", 50m, 1);
            SetStatusPedido(pedido, status);

            //Act
            Action act = () => pedido.RemoverItem(pedido.Itens.First().Id);

            //Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Itens só podem ser removidos em pedidos pendentes.");
        }

        [Fact(DisplayName = "Deve atualizar endereço de entrega em pedido Pendente")]
        public void Deve_Atualizar_Endereco_Entrega_Em_Pedido_Pendente()
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            var novoEndereco = EnderecoEntrega.Criar(
                cep: "98765-432",
                logradouro: "Avenida Nova, 456",
                complemento: "Casa",
                bairro: "Bairro Novo",
                estado: "RJ",
                cidade: "Rio de Janeiro",
                pais: "Brasil"
            );

            //Act
            pedido.AtualizarEnderecoEntrega(novoEndereco);

            //Assert
            pedido.EnderecoEntrega.Should().Be(novoEndereco);
        }

        [Theory(DisplayName = "Não deve permitir atualizar endereço quando pedido não estiver Pendente")]
        [InlineData(StatusPedido.PagamentoConfirmado)]
        [InlineData(StatusPedido.EmSeparacao)]
        [InlineData(StatusPedido.Enviado)]
        [InlineData(StatusPedido.Entregue)]
        [InlineData(StatusPedido.Cancelado)]
        public void Nao_Deve_Atualizar_Endereco_Se_Pedido_Nao_Estiver_Pendente(StatusPedido status)
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            var novoEndereco = EnderecoEntrega.Criar(
                cep: "98765-432",
                logradouro: "Avenida Nova, 456",
                complemento: "Casa",
                bairro: "Bairro Novo",
                estado: "RJ",
                cidade: "Rio de Janeiro",
                pais: "Brasil"
            );
            SetStatusPedido(pedido, status);

            //Act
            Action act = () => pedido.AtualizarEnderecoEntrega(novoEndereco);

            //Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Endereço só pode ser alterado enquanto o pedido está pendente.");
        }

        [Fact(DisplayName = "Deve iniciar pagamento e manter status Pendente")]
        public void Deve_Iniciar_Pagamento_E_Manter_Status_Pendente()
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Produto Exemplo", 100m, 1);

            //Act
            var pagamento = pedido.IniciarPagamento(MetodoPagamento.CartaoCredito);

            //Assert
            pagamento.Should().NotBeNull();
            pagamento.Valor.Should().Be(100m);
            pedido.Pagamentos.Should().Contain(pagamento);
            pedido.StatusPedido.Should().Be(StatusPedido.Pendente);
        }

        [Fact(DisplayName = "Não deve iniciar pagamento em pedido sem itens")]
        public void Nao_Deve_Iniciar_Pagamento_Em_Pedido_Sem_Itens()
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());

            //Act
            Action act = () => pedido.IniciarPagamento(MetodoPagamento.CartaoCredito);

            //Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Não é possível iniciar o pagamento de um pedido sem itens.");
        }

        [Fact(DisplayName = "Não deve iniciar pagamento se já houver um pagamento Pendente")]
        public void Nao_Deve_Iniciar_Pagamento_Se_Ja_Houver_Um_Pagamento_Pendente()
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Produto Exemplo", 100m, 1);
            pedido.IniciarPagamento(MetodoPagamento.CartaoCredito);

            //Act
            Action act = () => pedido.IniciarPagamento(MetodoPagamento.Boleto);

            //Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Já existe um pagamento pendente para este pedido.");
        }

        [Fact(DisplayName = "Deve alterar status para PagamentoConfirmado ao HandlePagamentoAprovado")]
        public void Deve_Alterar_Status_Para_PagamentoConfirmado_Ao_HandlePagamentoAprovado()
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Produto Exemplo", 100m, 1);
            var pagamento = pedido.IniciarPagamento(MetodoPagamento.CartaoCredito);

            //Act
            pedido.HandlePagamentoAprovado(pagamento.Id);

            //Assert
            pedido.StatusPedido.Should().Be(StatusPedido.PagamentoConfirmado);
        }

        [Fact(DisplayName = "Deve manter status Pendente ao HandlePagamentoRejeitado")]
        public void Deve_Manter_Status_Pendente_Ao_HandlePagamentoRecusado()
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Produto Exemplo", 100m, 1);
            var pagamento = pedido.IniciarPagamento(MetodoPagamento.CartaoCredito);

            //Act
            pedido.HandlePagamentoRejeitado(pagamento.Id);

            //Assert
            pedido.StatusPedido.Should().Be(StatusPedido.Cancelado);
            pedido.DomainEvents.Should().ContainSingle(e => e is PedidoCanceladoEvent);
        }

        [Fact(DisplayName = "Não deve HandlePagamentoAprovado se status não for Pendente")]
        public void Nao_Deve_HandlePagamentoAprovado_Se_Status_Nao_For_Pendente()
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Produto Exemplo", 100m, 1);
            var pagamento = pedido.IniciarPagamento(MetodoPagamento.CartaoCredito);
            SetStatusPedido(pedido, StatusPedido.EmSeparacao);

            //Act
            Action act = () => pedido.HandlePagamentoAprovado(pagamento.Id);

            //Assert
            act.Should().Throw<DomainException>()
                .WithMessage("O pedido não está no status esperado para confirmação de pagamento");
        }

        [Fact(DisplayName = "Deve permitir marcar pedido como Em Separacao após PagamentoConfirmado")]
        public void Deve_Marcar_Como_Em_Separacao()
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Produto Exemplo", 100m, 1);
            var pagamento = pedido.IniciarPagamento(MetodoPagamento.CartaoCredito);
            pedido.HandlePagamentoAprovado(pagamento.Id);

            //Act
            pedido.MarcarComoEmSeparacao();

            //Assert
            pedido.StatusPedido.Should().Be(StatusPedido.EmSeparacao);
        }

        [Fact(DisplayName = "Não deve permitir marcar como Em Separacao se status não for PagamentoConfirmado")]
        public void Nao_Deve_Marcar_Como_Em_Separacao_Se_Status_Nao_For_PagamentoConfirmado()
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());

            //Act
            Action act = () => pedido.MarcarComoEmSeparacao();

            //Assert
            act.Should().Throw<DomainException>()
                .WithMessage("O pedido só pode ir para 'Em Separação' após o pagamento ser confirmado.");
        }

        [Fact(DisplayName = "Deve permitir marcar pedido como Enviado após EmSeparacao")]
        public void Deve_Marcar_Como_Enviado()
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            SetStatusPedido(pedido, StatusPedido.EmSeparacao);

            //Act
            pedido.MarcarComoEnviado();

            //Assert
            pedido.StatusPedido.Should().Be(StatusPedido.Enviado);
        }

        [Fact(DisplayName = "Não deve permitir marcar como Enviado se status não for EmSeparacao")]
        public void Nao_Deve_Marcar_Como_Enviado_Se_Status_Nao_For_EmSeparacao()
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            SetStatusPedido(pedido, StatusPedido.PagamentoConfirmado);

            //Act
            Action act = () => pedido.MarcarComoEnviado();

            //Assert
            act.Should().Throw<DomainException>()
                .WithMessage("O pedido só pode ser marcado como 'Enviado' se estiver 'Em Separação'.");
        }

        [Fact(DisplayName = "Deve marcar pedido como Entregue")]
        public void Deve_Marcar_Como_Entregue()
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            SetStatusPedido(pedido, StatusPedido.Enviado);

            //Act
            pedido.MarcarComoEntregue();

            //Assert
            pedido.StatusPedido.Should().Be(StatusPedido.Entregue);
        }

        [Fact(DisplayName = "Não deve marcar como Entregue se não estiver Enviado")]
        public void Nao_Deve_Marcar_Como_Entregue_Se_Nao_Enviado()
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            SetStatusPedido(pedido, StatusPedido.EmSeparacao);

            //Act
            Action act = () => pedido.MarcarComoEntregue();

            //Assert
            act.Should().Throw<DomainException>()
                .WithMessage("O pedido só pode ser marcado como 'Entregue' se estiver 'Enviado'.");
        }

        [Fact(DisplayName = "Deve cancelar pedido se estiver Pendente")]
        public void Deve_Cancelar_Pedido_Se_Estiver_Pendente()
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Produto", 50m, 1);

            //Act
            pedido.CancelarPedido();

            //Assert
            pedido.StatusPedido.Should().Be(StatusPedido.Cancelado);
        }

        [Fact(DisplayName = "Deve cancelar pedido PagamentoConfirmado")]
        public void Deve_Cancelar_Pedido_PagamentoConfirmado()
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Produto", 50m, 1);
            var pagamento = pedido.IniciarPagamento(MetodoPagamento.CartaoCredito);
            pedido.HandlePagamentoAprovado(pagamento.Id);

            //Act
            pedido.CancelarPedido();

            //Assert
            pedido.StatusPedido.Should().Be(StatusPedido.Cancelado);
        }

        [Theory(DisplayName = "Não deve cancelar pedido se estiver EmSeparacao, Enviado ou Entregue")]
        [InlineData(StatusPedido.EmSeparacao)]
        [InlineData(StatusPedido.Enviado)]
        [InlineData(StatusPedido.Entregue)]
        public void Nao_Deve_Cancelar_Pedido_Se_Estiver_EmSeparacao_Enviado_Entregue(StatusPedido status)
        {
            //Arrange
            var pedido = Pedido.Criar(ClienteIdValido, CriarEnderecoValido());
            pedido.AdicionarItem(ProdutoIdValido, "Produto", 50m, 1);
            SetStatusPedido(pedido, status);

            //Act
            Action act = () => pedido.CancelarPedido();

            //Assert
            act.Should().Throw<DomainException>()
                .WithMessage("Não é possível cancelar um pedido que já está em separação ou posterior.");
        }
    }
}
