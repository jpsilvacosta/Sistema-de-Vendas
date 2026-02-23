using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Common.Validations;

namespace Vendas.Domain.Pedidos.Integration.Clientes
{
    public sealed class ClientesAcl
    {
        private readonly IClientesGateway _gateway;

        public ClientesAcl(IClientesGateway gateway)
        {
            Guard.AgainstNull(gateway, nameof(gateway));
            _gateway = gateway;
        }

        public async Task<EnderecoEntregaSnapshot> ObterEnderecoEntregaSnapshotAsync(
            Guid clienteId, 
            Guid enderecoId, 
            CancellationToken cancellationToken = default)
        {
            var dto = await _gateway.ObterEnderecoAsync(clienteId, enderecoId, cancellationToken);

            if (dto is null)
                throw new DomainException("Endereço não encontrado no contexto Clientes.");

            return new EnderecoEntregaSnapshot(
                dto.Cep,
                dto.Logradouro,
                dto.Numero,
                dto.Bairro,
                dto.Cidade,
                dto.Estado,
                dto.Pais,
                dto.Complemento);
        }
    }
}
