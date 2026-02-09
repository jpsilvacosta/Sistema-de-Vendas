using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Application.Commands.PedidosCommands.AtualizarEnderecoEntrega
{
    public sealed class AtualizarEnderecoEntregaCommand
    {
        public Guid PedidoId { get; }
        public EnderecoEntrega NovoEnderecoEntrega { get; }

        public AtualizarEnderecoEntregaCommand(Guid pedidoId, EnderecoEntrega novoEnderecoEntrega)
        {
            PedidoId = pedidoId;
            NovoEnderecoEntrega = novoEnderecoEntrega;
        }
    }
}
