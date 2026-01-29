namespace Vendas.Domain.Pedidos.Events
{
    public interface IDomainEvent
    {
        DateTime DateOccurred { get; }
    }
}
