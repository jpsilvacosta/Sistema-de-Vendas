namespace Vendas.Application.Commands.Pedidos.CancelarPedido
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
