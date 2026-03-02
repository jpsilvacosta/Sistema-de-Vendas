namespace Vendas.Application.Commands.ClientesCommands.AdicionarEnderecoAoCliente
{
    public sealed class AdicionarEnderecoAoClienteCommand
    {
        public Guid ClienteId { get; }
        public Guid EnderecoId { get; }

        public AdicionarEnderecoAoClienteCommand(Guid clienteId, Guid enderecoId)
        {
            ClienteId = clienteId;
            EnderecoId = enderecoId;
        }
    }
}
