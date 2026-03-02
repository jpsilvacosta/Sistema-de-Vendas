using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Application.Commands.PedidosCommands.CriarPedido
{
    public sealed class CriarPedidoCommand
    {
        public Guid ClienteId { get; }
        public Guid EnderecoId { get; }

        public CriarPedidoCommand(Guid clienteId, Guid enderecoId)
        {
            ClienteId = clienteId;
            EnderecoId = enderecoId;
        }
    }
}
