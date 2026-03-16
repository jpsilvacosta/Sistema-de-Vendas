using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Pedidos;

namespace Vendas.Infra.Fakes
{
    public sealed class FakePedidoRepository : IPedidoRepository
    {
        private readonly Dictionary<Guid, Pedido> _pedidos = new();
        private readonly SemaphoreSlim _lock = new(1, 1);

        public async Task<Pedido?> ObterPorIdAsync(Guid pedidoId, CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(cancellationToken);
            try
            {
                _pedidos.TryGetValue(pedidoId, out var pedido);
                return pedido;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task AdicionarAsync(Pedido pedido, CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(cancellationToken);
            try
            {
                _pedidos[pedido.Id] = pedido;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task AtualizarAsync(Pedido pedido, CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(cancellationToken);
            try
            {
                if (!_pedidos.ContainsKey(pedido.Id))
                   throw new InvalidOperationException($"Pedido {pedido.Id} não encontrado para atualização.");

                _pedidos[pedido.Id] = pedido;
            }
            finally
            {
                _lock.Release();
            }
        }

        //Método auxiliar para listar todos os pedidos (uso nos endpoints de consulta)

        public async Task<IReadOnlyList<Pedido>> ListarTodosAsync(CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(cancellationToken);
            try
            {
                return _pedidos.Values.ToList().AsReadOnly();
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
