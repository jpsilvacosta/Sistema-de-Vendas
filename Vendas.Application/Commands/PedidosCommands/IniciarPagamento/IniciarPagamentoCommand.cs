using Vendas.Domain.Pedidos.Enums;

namespace Vendas.Application.Commands.PedidosCommands.IniciarPagamento
{
    public sealed class IniciarPagamentoCommand
    {
        public Guid PedidoId { get; }
        public MetodoPagamento MetodoPagamento {  get; }

        public IniciarPagamentoCommand(Guid pedidoId, MetodoPagamento metodoPagamento)
        {
            PedidoId = pedidoId;
            MetodoPagamento = metodoPagamento;
        }
    }
}
