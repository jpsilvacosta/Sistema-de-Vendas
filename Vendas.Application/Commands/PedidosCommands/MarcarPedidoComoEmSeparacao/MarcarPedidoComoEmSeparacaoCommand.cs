namespace Vendas.Application.Commands.PedidosCommands.MarcarPedidoComoEmSeparacao
{
    public sealed class MarcarPedidoComoEmSeparacaoCommand
    {
        public Guid PedidoId { get; }

        public MarcarPedidoComoEmSeparacaoCommand(Guid pedidoId)
        {
            PedidoId = pedidoId;
        }
    }
}
