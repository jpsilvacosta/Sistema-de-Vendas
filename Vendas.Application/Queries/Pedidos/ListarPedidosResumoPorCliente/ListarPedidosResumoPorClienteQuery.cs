namespace Vendas.Application.Queries.Pedidos.ListarPedidosResumoPorCliente
{
    public sealed class ListarPedidosResumoPorClienteQuery
    {
        public Guid ClienteId { get; }

        public ListarPedidosResumoPorClienteQuery(Guid clienteId)
        {
            if(clienteId == Guid.Empty)
                throw new ArgumentException("ClienteId inválido.", nameof(clienteId));

            ClienteId = clienteId;
        }
    }
}
