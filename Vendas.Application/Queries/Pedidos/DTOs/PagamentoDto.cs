namespace Vendas.Application.Queries.Pedidos.DTOs
{
    public sealed class PagamentoDto
    {
        public Guid PagamentoId { get; init; }
        public string MetodoPagamento { get; init; } = string.Empty;
        public string StatusPagamento { get; init; } = string.Empty;
        public decimal Valor { get; init; }
        public string? CodigoTransacao { get; init; }
        public DateTime? DataPagamento { get; init; }
    }
}
