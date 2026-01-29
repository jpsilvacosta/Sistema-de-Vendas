using FluentAssertions;
using Vendas.Domain.Clientes.Entities;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Domain.Tests.Clientes.Entities
{
    public class EnderecoTests
    {
        private static Endereco CriarEnderecoValido()
        {
            return new Endereco(
                cep: "12345678",
                logradouro: "Rua A",
                numero: "100",
                bairro: "Centro",
                cidade: "São Paulo",
                estado: "SP",
                pais: "Brasil"
                );
        }

        [Fact(DisplayName = "Deve criar endereço válido")]
        public void Deve_Criar_Endereco_Valido()
        {
            //Arrange & Act
            var endereco = CriarEnderecoValido();

            //Assert
            endereco.Cep.Should().Be("12345678");
            endereco.Logradouro.Should().Be("Rua A");
            endereco.Numero.Should().Be("100");
            endereco.Bairro.Should().Be("Centro");
            endereco.Cidade.Should().Be("São Paulo");
            endereco.Estado.Should().Be("SP");
            endereco.Pais.Should().Be("Brasil");
            endereco.Complemento.Should().BeEmpty();
        }

        [Theory(DisplayName = "Deve lançar erro quando o CEP for inválido")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Deve_Lancar_Erro_Quando_CEP_For_Invalido(string? cepInvalido)
        {
            //Arrange & Act
            Action act = () => new Endereco(
                cep: cepInvalido!,
                logradouro: "Rua A",
                numero: "100",
                bairro: "Centro",
                cidade: "São Paulo",
                estado: "SP",
                pais: "Brasil"
                );

            //Assert
            act.Should().Throw<DomainException>()
                .WithMessage("O CEP é obrigatório.");
        }

        [Fact]
        public void Deve_Lancar_Erro_Quando_CEP_Nao_Tiver_8_Digitos()
        {
            //Arrange & Act
            Action act = () => new Endereco(
                cep: "1234",
                logradouro: "Rua A",
                numero: "100",
                bairro: "Centro",
                cidade: "São Paulo",
                estado: "SP",
                pais: "Brasil"
                );

            //Assert
            act.Should().Throw<DomainException>()
                .WithMessage("CEP inválido.");
        }

        [Theory(DisplayName = "Deve lançar erro quando campos obrigatórios forem inválidos")]
        [InlineData(null, "100", "Centro","São Paulo","SP","Brasil")]
        [InlineData("Rua A", null, "Centro","São Paulo","SP","Brasil")]
        [InlineData("Rua A", "100", null, "São Paulo","SP","Brasil")]
        [InlineData("Rua A", "100", "Centro", null, "SP","Brasil")]
        [InlineData("Rua A", "100", "Centro","São Paulo", null, "Brasil")]
        [InlineData("Rua A", "100", "Centro","São Paulo", "SP", null)]
        public void Deve_Lancar_Erro_Quando_Campos_Obrigatorios_Forem_Invalidos(
            string? logradouro,
            string? numero,
            string? bairro,
            string? cidade,
            string? estado,
            string? pais)
        {
            //Arrange & Act
            Action act = () => new Endereco(
                cep: "12345678",
                logradouro = logradouro!,
                numero = numero!,
                bairro = bairro!,
                cidade = cidade!,
                estado = estado!,
                pais = pais!
                );

            //Assert
            act.Should().Throw<DomainException>();
        }

    }
}
