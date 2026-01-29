using FluentAssertions;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Pedidos.Entities;
using Vendas.Domain.Pedidos.Enums;
using Vendas.Domain.Pedidos.Events;

namespace Vendas.Domain.Tests.Pedidos.Entities
{
    public class PagamentoTests
    {
        [Fact(DisplayName = "Deve criar um pagamento válido com status pendente")]
        public void Deve_Criar_Pagamento_Valido_Com_Status_Pendente()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();
            var metodo = MetodoPagamento.CartaoCredito;
            var valor = 100m;

            // Act
            var pagamento = new Pagamento(pedidoId, metodo, valor);

            // Assert
            pagamento.PedidoId.Should().Be(pedidoId);
            pagamento.MetodoPagamento.Should().Be(metodo);
            pagamento.Valor.Should().Be(valor);
            pagamento.StatusPagamento.Should().Be(StatusPagamento.Pendente);
            pagamento.DataPagamento.Should().BeNull();
            pagamento.CodigoTransacao.Should().BeNull();
        }

        [Fact(DisplayName = "Não deve criar pagamento com valor inválido")]
        public void Nao_Deve_Criar_Pagamento_Com_Valor_Invalido()
        {
            // Arrange
            var pedidoId = Guid.NewGuid();

            // Act
            Action act = () => new Pagamento(pedidoId, MetodoPagamento.Pix, 0);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("O valor do pagamento deve ser maior que zero.");
        }

        [Fact(DisplayName = "Não deve definir o código de transação para nulo ou vazio")]
        public void Nao_Deve_Definir_Codigo_Transacao_Para_Nulo_Ou_Vazio()
        {
            // Arrange
            var pagamento = new Pagamento(Guid.NewGuid(), MetodoPagamento.Pix, 100m);

            // Act
            Action act = () => pagamento.DefinirCodigoTransacao("");

            // Assert
            act.Should()
                .Throw<DomainException>()
                .WithMessage("Código de transação inválido.");
        }

        [Fact(DisplayName = "Deve definir código de transação válido e atualizar DataAtualizacao")]
        public void Deve_Definir_Codigo_Transacao_Valido()
        {
            // Arrange
            var pagamento = new Pagamento(Guid.NewGuid(), MetodoPagamento.Pix, 100m);
            var codigoTransacao = "TXN-12345";

            // Act
            pagamento.DefinirCodigoTransacao(codigoTransacao);

            // Assert
            pagamento.CodigoTransacao.Should().Be(codigoTransacao);
            pagamento.DataAtualizacao.Should().NotBeNull();
        }

        [Fact(DisplayName = "Não deve redefinir código de transação já definido")]
        public void Nao_Deve_Redefinir_Codigo_Transacao_Ja_Definido()
        {
            // Arrange
            var pagamento = new Pagamento(Guid.NewGuid(), MetodoPagamento.Pix, 100m);
            var codigoTransacao = "TXN-12345";
            pagamento.DefinirCodigoTransacao(codigoTransacao);

            // Act
            Action act = () => pagamento.DefinirCodigoTransacao("TXN-67890");

            // Assert
            act.Should()
                .Throw<DomainException>()
                .WithMessage("O código de transação já foi definido.");
        }

        [Fact(DisplayName = "Deve gerar código de transação local automaticamente")]
        public void Deve_Gerar_Codigo_Transacao_Local()
        {
            // Arrange
            var pagamento = new Pagamento(Guid.NewGuid(), MetodoPagamento.Pix, 100m);

            // Act
            pagamento.GerarCodigoTransacaoLocal();

            // Assert
            pagamento.CodigoTransacao.Should().StartWith("LOCAL-");
            pagamento.CodigoTransacao.Should().HaveLength(14);
            pagamento.DataAtualizacao.Should().NotBeNull();
        }

        [Fact(DisplayName = "Deve confirmar pagamento pendente com código válido e gerar evento completo")]
        public void Deve_Confirmar_Pagamento_Pendente_Com_Codigo_Valido_E_Evento_Completo()
        {
            // Arrange
            var pagamento = new Pagamento(Guid.NewGuid(), MetodoPagamento.CartaoCredito, 150m);
            pagamento.GerarCodigoTransacaoLocal();

            // Act
            pagamento.ConfirmarPagamento();

            // Assert
            pagamento.StatusPagamento.Should().Be(StatusPagamento.Aprovado);
            pagamento.DataPagamento.Should().NotBeNull();
            pagamento.DataAtualizacao.Should().NotBeNull();

            var evento = pagamento.DomainEvents.OfType<PagamentoAprovadoEvent>().FirstOrDefault();
            evento.Should().NotBeNull();
            evento!.PagamentoId.Should().Be(pagamento.Id);
            evento.PedidoId.Should().Be(pagamento.PedidoId);
            evento.Valor.Should().Be(pagamento.Valor);
            evento.DataPagamento.Should().Be(pagamento.DataPagamento);
            evento.CodigoTransacao.Should().Be(pagamento.CodigoTransacao);
        }

        [Fact(DisplayName = "Não deve confirmar pagamento sem código de transação")]
        public void Nao_Deve_Confirmar_Pagamento_Sem_Codigo_De_Transacao()
        {
            // Arrange
            var pagamento = new Pagamento(Guid.NewGuid(), MetodoPagamento.CartaoCredito, 150m);

            // Act
            Action act = () => pagamento.ConfirmarPagamento();

            // Assert
            act.Should()
                .Throw<DomainException>()
                .WithMessage("O pagamento não pode ser confirmado sem o código de transação.");
        }

        [Fact(DisplayName = "Não deve confirmar pagamento que não está pendente")]
        public void Nao_Deve_Confirmar_Pagamento_Que_Nao_Esta_Pendente()
        {
            // Arrange
            var pagamento = new Pagamento(Guid.NewGuid(), MetodoPagamento.CartaoCredito, 150m);
            pagamento.GerarCodigoTransacaoLocal();
            pagamento.ConfirmarPagamento();

            // Act
            Action act = () => pagamento.ConfirmarPagamento();

            // Assert
            act.Should()
                .Throw<DomainException>()
                .WithMessage("Apenas pagamentos pendentes podem ser confirmados.");
        }

        [Fact(DisplayName = "Deve recusar pagamento pendente e gerar evento de rejeição com dados corretos")]
        public void Deve_Recusar_Pagamento_Pendente_E_Gerar_Evento_De_Rejeicao_Com_Dados_Corretos()
        {
            // Arrange
            var pagamento = new Pagamento(Guid.NewGuid(), MetodoPagamento.Boleto, 200m);

            // Act
            pagamento.RecusarPagamento();

            // Assert
            pagamento.StatusPagamento.Should().Be(StatusPagamento.Recusado);
            pagamento.DataPagamento.Should().NotBeNull();
            pagamento.DataAtualizacao.Should().NotBeNull();

            var evento = pagamento.DomainEvents.OfType<PagamentoRejeitadoEvent>().FirstOrDefault();
            evento.Should().NotBeNull();
            evento!.PagamentoId.Should().Be(pagamento.Id);
            evento.PedidoId.Should().Be(pagamento.PedidoId);
            evento.Valor.Should().Be(pagamento.Valor);
            evento.CodigoTransacao.Should().Be(pagamento.CodigoTransacao);
            evento.DataPagamento.Should().Be(pagamento.DataPagamento);
        }

        [Fact(DisplayName = "Não deve recusar pagamento que não está pendente")]
        public void Nao_Deve_Recusar_Pagamento_Que_Nao_Esta_Pendente()
        {
            // Arrange
            var pagamento = new Pagamento(Guid.NewGuid(), MetodoPagamento.Boleto, 200m);
            pagamento.GerarCodigoTransacaoLocal();
            pagamento.ConfirmarPagamento();

            // Act
            Action act = () => pagamento.RecusarPagamento();

            // Assert
            act.Should()
                .Throw<DomainException>()
                .WithMessage("Apenas pagamentos pendentes podem ser recusados.");
        }
    }
}
