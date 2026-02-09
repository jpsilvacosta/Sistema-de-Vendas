using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Application.Commands.PedidosCommands.AtualizarEnderecoEntrega
{
    public sealed class AtualizarEnderecoEntregaResultDto
    {
        public Guid PedidoId { get; }
        public string EnderecoEntrega { get; }
        public string Status {  get; }

        public AtualizarEnderecoEntregaResultDto(Guid pedidoId, string enderecoEntrega, string status  )
        {
            PedidoId = pedidoId;
            EnderecoEntrega = enderecoEntrega;
            Status = status;
        }
    }
}
