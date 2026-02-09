using FluentAssertions;
using Vendas.Domain.Clientes;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Domain.Tests.Clientes
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
             
            var endereco = CriarEnderecoValido();

            
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
             
            Action act = () => new Endereco(
                cep: cepInvalido!,
                logradouro: "Rua A",
                numero: "100",
                bairro: "Centro",
                cidade: "São Paulo",
                estado: "SP",
                pais: "Brasil"
                );

            
            act.Should().Throw<DomainException>()
                .WithMessage("O CEP é obrigatório.");
        }

        [Fact]
        public void Deve_Lancar_Erro_Quando_CEP_Nao_Tiver_8_Digitos()
        {
             
            Action act = () => new Endereco(
                cep: "1234",
                logradouro: "Rua A",
                numero: "100",
                bairro: "Centro",
                cidade: "São Paulo",
                estado: "SP",
                pais: "Brasil"
                );

            
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
             
            Action act = () => new Endereco(
                cep: "12345678",
                logradouro = logradouro!,
                numero = numero!,
                bairro = bairro!,
                cidade = cidade!,
                estado = estado!,
                pais = pais!
                );

            
            act.Should().Throw<DomainException>();
        }

        [Fact(DisplayName = "Deve atualizar endereço com dados válidos")]
        public void Deve_Atualizar_Endereco_Com_Dados_Validos()
        {
            
            var endereco = CriarEnderecoValido();

            
            endereco.Atualizar(
                cep: "87654321",
                logradouro: "Rua B",
                numero: "200",
                bairro: "Leste",
                cidade: "Rio de Janeiro",
                estado: "RJ",
                pais: "Brasil",
                complemento: "Apto 121"
                );

            
            endereco.Cep.Should().Be("87654321");
            endereco.Logradouro.Should().Be("Rua B");
            endereco.Numero.Should().Be("200");
            endereco.Bairro.Should().Be("Leste");
            endereco.Cidade.Should().Be("Rio de Janeiro");
            endereco.Estado.Should().Be("RJ");
            endereco.Pais.Should().Be("Brasil");
            endereco.Complemento.Should().Be("Apto 121");
        }

        [Fact(DisplayName = "Deve lançar erro ao atualizar com CEP inválido")]
        public void Deve_Lancar_Erro_Ao_Atualizar_Com_CEP_Invalido()
        {
            
            var endereco = CriarEnderecoValido();

            
            Action act = () => endereco.Atualizar(
                cep: "123",
                logradouro: "Rua Teste",
                numero: "10",
                bairro: "Centro",
                cidade: "SP",
                estado: "SP",
                pais: "Brasil"
                );

            
            act.Should().Throw<DomainException>()
                .WithMessage("CEP inválido.");
        }

        [Fact(DisplayName = "Deve lançar erro ao atualizar com campo obrigatório inválido.")]
        public void Deve_Lancar_Erro_Ao_Atualizar_Com_Campo_Obrigatorio_Invalido()
        {
            
            var endereco = CriarEnderecoValido();

            
            Action act = () => endereco.Atualizar(
                cep: "12345678",
                logradouro: "",
                numero: "10",
                bairro: "Centro",
                cidade: "SP",
                estado: "SP",
                pais: "Brasil"
                );

            
            act.Should().Throw<DomainException>();
        }
    }
}
