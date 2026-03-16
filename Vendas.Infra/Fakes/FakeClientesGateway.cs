using Vendas.Domain.Pedidos.Integration.Clientes;

namespace Vendas.Infra.Fakes
{
    public sealed class FakeClientesGateway : IClientesGateway
    {
        private static readonly Dictionary<Guid, Dictionary<Guid, EnderecoDto>> _clientes = new()
        {
            [new Guid("22222222-0000-0000-0000-000000000001")] = new()
            {
                [new Guid("33333333-0000-0000-0000-000000000001")] = new(
                    id: new Guid("33333333-0000-0000-0000-000000000001"),
                    cep: "12345-678", logradouro: "Avenida Paulista", numero: "1000", bairro: "Bela Vista", cidade: "São Paulo", estado: "SP", pais: "Brasil", complemento: "Apto 101"
                ),
                [new Guid("33333333-0000-0000-0000-000000000002")] = new(
                    id: new Guid("33333333-0000-0000-0000-000000000002"),
                    cep: "98765-432", logradouro: "Rua das Flores", numero: "200", bairro: "Jardim", cidade: "Rio de Janeiro", estado: "RJ", pais: "Brasil", complemento: "Casa"
                ),
                [new Guid("44444444-0000-0000-0000-000000000003")] = new(
                    id: new Guid("44444444-0000-0000-0000-000000000003"),
                    cep: "54321-987", logradouro: "Praça Central", numero: "50", bairro: "Centro", cidade: "Belo Horizonte", estado: "MG", pais: "Brasil", complemento: "Sala 202"
                )
            }
        };

        public Task<EnderecoDto?> ObterEnderecoAsync(Guid clienteId, Guid enderecoId, CancellationToken cancellationToken = default)
        {
            if(_clientes.TryGetValue(clienteId, out var enderecos) && enderecos.TryGetValue(enderecoId, out var endereco))
            {
                return Task.FromResult<EnderecoDto?>(endereco);
            }
            return Task.FromResult<EnderecoDto?>(null);
        }
    }
}
