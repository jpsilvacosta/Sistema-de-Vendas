using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Application.Commands.PedidosCommands.MarcarPedidoComoEntregue
{
    public sealed class MarcarPedidoComoEntregueCommandHandler
    {
        private readonly IPedidoRepository _pedidoRepository;

        public MarcarPedidoComoEntregueCommandHandler(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public async Task<MarcarPedidoComoEntregueResultDto> HandleAsync(MarcarPedidoComoEntregueCommand command, CancellationToken cancellationToken = default)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(command.PedidoId, cancellationToken)
                ?? throw new DomainException("Pedido não encontrado");

            pedido.MarcarComoEntregue();

            await _pedidoRepository.AtualizarAsync(pedido, cancellationToken);

            return new MarcarPedidoComoEntregueResultDto
            {
                PedidoId = pedido.Id,
                StatusPedido = pedido.StatusPedido.ToString()
            };
        }
    }
}
