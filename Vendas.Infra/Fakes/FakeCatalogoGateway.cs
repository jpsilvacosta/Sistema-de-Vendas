using Vendas.Domain.Pedidos.Integration.Catalogo;

namespace Vendas.Infra.Fakes
{
    public sealed class FakeCatalogoGateway : ICatalogoGateway
    {
        private static readonly Dictionary<Guid, ProdutoDto> _produtos = new()
        {
            [new Guid("11111111-0000-0000-0000-000000000001")]  = new(new Guid("11111111-0000-0000-0000-000000000001"), "Notebook Gamer", 8500.00m),
            [new Guid("11111111-0000-0000-0000-000000000002")]  = new(new Guid("11111111-0000-0000-0000-000000000002"), "Smartphone", 2500.00m),
            [new Guid("11111111-0000-0000-0000-000000000003")]  = new(new Guid("11111111-0000-0000-0000-000000000003"), "Headset", 300.00m),
            [new Guid("11111111-0000-0000-0000-000000000004")]  = new(new Guid("11111111-0000-0000-0000-000000000004"), "Teclado Mecânico", 450.00m),
        };

        public Task<ProdutoDto?> ObterProdutoPorIdAsync(Guid produtoId, CancellationToken ct = default)
        {
            _produtos.TryGetValue(produtoId, out var produto);
            return Task.FromResult(produto);
        }
    }
}
