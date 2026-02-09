using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Application.Commands.PedidosCommands.CriarPedido
{
    public sealed class CriarPedidoCommand
    {
        public Guid ClienteId { get; }
        public EnderecoEntrega EnderecoEntrega { get; }

        public CriarPedidoCommand(Guid clienteId, EnderecoEntrega enderecoEntrega)
        {
            ClienteId = clienteId;
            EnderecoEntrega = enderecoEntrega;
        }
    }
}
