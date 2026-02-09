using Vendas.Application.Abstractions.Persistence;

namespace Vendas.Application.Commands.PedidosCommands.AdicionarItemAoPedido
{
    public sealed class AdicionarItemAoPedidoCommandHandler
    {
        private readonly IPedidoRepository _pedidoRepository;

        public AdicionarItemAoPedidoCommandHandler(IPedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }

        public async Task<AdicionarItemAoPedidoResultDto> HandleAsync(
            AdicionarItemAoPedidoCommand command, CancellationToken cancellationToken)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(command.PedidoId, cancellationToken);

            if (pedido is null)
                throw new InvalidOperationException("Pedido não encontrado.");

            pedido.AdicionarItem(command.ProdutoId, command.NomeProduto, command.PrecoUnitario, command.Quantidade);

            await _pedidoRepository.AtualizarAsync(pedido, cancellationToken);

            return new AdicionarItemAoPedidoResultDto(
                pedido.Id,
                pedido.ValorTotal,
                pedido.StatusPedido.ToString()
                );
        }
    }
}
