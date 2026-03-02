using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Pedidos;
using Vendas.Domain.Pedidos.Integration.Clientes;

namespace Vendas.Application.Commands.PedidosCommands.CriarPedido
{
    public sealed class CriarPedidoCommandHandler
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IClientesGateway _clientesGateway;
        private readonly ClientesAcl _clientesAcl;

        public CriarPedidoCommandHandler(IPedidoRepository pedidoRepository, IClientesGateway clientesGateway, ClientesAcl clientesAcl) 
        {
            _pedidoRepository = pedidoRepository;
            _clientesGateway = clientesGateway;
            _clientesAcl = clientesAcl;
        }

        public async Task<CriarPedidoResultDto> HandleAsync(CriarPedidoCommand command, CancellationToken cancellationToken = default) {
            var enderecoDto = await _clientesGateway.ObterEnderecoAsync(command.ClienteId, command.EnderecoId, cancellationToken);

            if (enderecoDto is null)
                throw new InvalidOperationException("Endereço não encontrado");

            var enderecoEntrega = _clientesAcl.TraduzirEndereco(enderecoDto);

            var pedido = Pedido.Criar(command.ClienteId, enderecoEntrega);

            await _pedidoRepository.AdicionarAsync(pedido, cancellationToken);

            return new CriarPedidoResultDto(pedido.Id, pedido.NumeroPedido, pedido.DataCriacao, pedido.ValorTotal, pedido.StatusPedido.ToString());
        }
    }
}
