namespace Vendas.Domain.Events
{
    public sealed record PedidoEntregueEvent(Guid PedidoId, Guid ClienteId) : DomainEventBase;
}
