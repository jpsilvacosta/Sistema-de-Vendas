namespace Vendas.Application.Commands.PedidosCommands.MarcarPedidoComoEnviado
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
