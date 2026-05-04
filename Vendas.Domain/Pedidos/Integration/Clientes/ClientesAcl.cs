using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Domain.Pedidos.Integration.Clientes
{
    public sealed class ClientesAcl
    {
        public EnderecoEntrega TraduzirEndereco(EnderecoDto dto)
        {
            return EnderecoEntrega.Criar(
            cep: dto.Cep,
            logradouro: dto.Logradouro,
            numero: dto.Numero,
            complemento: dto.Complemento,
            bairro: dto.Bairro,
            estado: dto.Estado,
            cidade: dto.Cidade,
            pais: dto.Pais);
        }
    }
}