using System.Collections.ObjectModel;
using Vendas.Application.Abstractions.Persistence;
using Vendas.Domain.Clientes;
using Vendas.Domain.Pedidos.Integration.Clientes;

namespace Vendas.Application.Commands.ClientesCommands.AdicionarEnderecoAoCliente
{
    public sealed class AdicionarEnderecoAoClienteCommandHandler
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly ClientesAcl _clientesAcl;
        private readonly IClientesGateway _clientesGateway;

        public AdicionarEnderecoAoClienteCommandHandler(IClienteRepository clienteRepository, ClientesAcl clientesAcl, IClientesGateway clientesGateway)
        {
            _clienteRepository = clienteRepository;
            _clientesAcl = clientesAcl;
            _clientesGateway = clientesGateway;
        }

        //public async Task<AdicionarEnderecoAoClienteResultDto> HandleAsync(AdicionarEnderecoAoClienteCommand command, CancellationToken cancellationToken = default) {

        //    var enderecoDto = await _clientesGateway.ObterEnderecoAsync(command.ClienteId, command.EnderecoId, cancellationToken);

        //    if (enderecoDto is null)
        //        throw new InvalidOperationException("Endereço não encontrado.");

        //    var endereco = _clientesAcl.TraduzirEndereco(enderecoDto);

        //    var cliente = Cliente.AdicionarEndereco(endereco);

        //}
    }
}
