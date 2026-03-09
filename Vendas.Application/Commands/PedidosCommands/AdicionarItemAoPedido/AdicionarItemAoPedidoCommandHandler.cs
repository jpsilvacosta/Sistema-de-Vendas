using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Pedidos.Integration.Catalogo;

namespace Vendas.Application.Commands.PedidosCommands.AdicionarItemAoPedido
{
    public sealed class AdicionarItemAoPedidoCommandHandler
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ICatalogoGateway _catalogoGateway;
        private readonly CatalogoAcl _catalogoAcl;

        public AdicionarItemAoPedidoCommandHandler(IPedidoRepository pedidoRepository, ICatalogoGateway catalogoGateway, CatalogoAcl catalogoAcl)
        {
            _pedidoRepository = pedidoRepository;
            _catalogoGateway = catalogoGateway;
            _catalogoAcl = catalogoAcl;
        }

        public async Task<AdicionarItemAoPedidoResultDto> HandleAsync(AdicionarItemAoPedidoCommand command, CancellationToken cancellationToken = default)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(command.PedidoId, cancellationToken);

            if (pedido is null)
                throw new InvalidOperationException("Pedido não encontrado.");

            var produtoDto = await _catalogoGateway.ObterProdutoPorIdAsync(command.ProdutoId, cancellationToken);

            if (produtoDto is null)
                throw new InvalidOperationException("Produto não encontrado.");

            var (nomeProduto, precoUnitario) = _catalogoAcl.TraduzirProduto(produtoDto);

            pedido.AdicionarItem(command.ProdutoId, nomeProduto, precoUnitario, command.Quantidade);

            await _pedidoRepository.AtualizarAsync(pedido, cancellationToken);

            return new AdicionarItemAoPedidoResultDto(pedido.Id, pedido.ValorTotal, pedido.StatusPedido.ToString());
        }
    }
}
