namespace Vendas.Application.Commands.Pedidos.MarcarPedidoComoEnviado
{
    public sealed class MarcarPedidoComoEnviadoResultDto
    {
        public Guid PedidoId { get; init; }
        public string StatusPedido { get; init; } = string.Empty;
    }
}
