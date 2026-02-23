namespace Vendas.Domain.Pedidos.Integration.Catalogo
{
    public interface ICatalogoGateway
    {
        Task<ProdutoDto?> ObterProdutoPorIdAsync(Guid produtoId, CancellationToken ct = default);

        Task<bool> PossuiEstoqueDisponivelAsync(Guid produtoId, int quantidade, CancellationToken ct = default);
    }
}
