namespace Vendas.Domain.Pedidos.Integration.Clientes
{
    public interface IClientesGateway
    {
        Task<EnderecoDto?> ObterEnderecoAsync(Guid clienteId, Guid enderecoId, CancellationToken cancellationToken = default);
    }
}
