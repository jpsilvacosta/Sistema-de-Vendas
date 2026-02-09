namespace Vendas.Application.Commands.PedidosCommands.CancelarPedido
{
    public sealed class CancelarPedidoCommand
    {
        public Guid PedidoId { get; }
        public string CodigoMotivo { get; }

        public CancelarPedidoCommand(Guid pedidoId, string codigoMotivo)
        {
            PedidoId = pedidoId;
            CodigoMotivo = codigoMotivo;
        }
    }
}
