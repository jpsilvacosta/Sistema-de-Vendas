namespace Vendas.Application.Commands.Pedidos.MarcarPedidoComoEnviado
{
    public sealed class MarcarPedidoComoEnviadoCommand
    {
        public Guid PedidoId { get; }

        public MarcarPedidoComoEnviadoCommand(Guid pedidoId)
        {
            PedidoId = pedidoId;
        }
    }
}
