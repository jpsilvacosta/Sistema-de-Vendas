using FluentAssertions;
using Vendas.Domain.Common.Exceptions;
using Vendas.Domain.Pedidos.ValueObjects;

namespace Vendas.Domain.Tests.Pedidos.ValueObjects
{
    public class EnderecoEntregaTests
    {
        [Fact(DisplayName = "Deve criar EnderecoEntrega com sucesso quando todos os dados são válidos")]
        public void Criar_DeveRetornarEnderecoValido_QuandoDadosForemValidos()
        {
            // Arrange
            var cep = "12345-678";
            var logradouro = "Rua das Flores";
            var numero = "100";
            var complemento = "Apto 101";
            var bairro = "Centro";
            var estado = "SP";
            var cidade = "São Paulo";
            var pais = "Brasil";

            // Act
            var endereco = EnderecoEntrega.Criar(cep, logradouro, numero, complemento, bairro, estado, cidade, pais);

            // Assert
            endereco.Should().NotBeNull();
            endereco.Cep.Should().Be(cep);
            endereco.Logradouro.Should().Be(logradouro);
            endereco.Numero.Should().Be(numero);
            endereco.Complemento.Should().Be(complemento);
            endereco.FormatarEndereco().Should().Contain("Rua das Flores");
        }

        [Theory(DisplayName = "Deve lançar DomainException quando o CEP for inválido")]
        [InlineData("12345678")]
        [InlineData("12-345678")]
        [InlineData("ABCDE-123")]
        public void Criar_DeveLancarDomainException_QuandoCepForInvalido(string cepInvalido)
        {
            // Arrange
            var logradouro = "Rua das Flores";
            var complemento = "Casa";
            var numero = "100";
            var bairro = "Centro";
            var estado = "SP";
            var cidade = "São Paulo";
            var pais = "Brasil";

            // Act
            Action act = () => EnderecoEntrega.Criar(cepInvalido, logradouro, numero, complemento, bairro, estado, cidade, pais);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("CEP inválido*");
        }

        [Fact(DisplayName = "Dois EnderecosEntrega com mesmos dados devem ser iguais (VOs)")]
        public void EnderecosDevemSerIguais_QuandoPossuemMesmosValores()
        {
            
            var endereco1 = EnderecoEntrega.Criar("12345-678", "Rua A","Numero 100", "Apto 1", "Bairro B", "SP", "Cidade C", "Brasil");
            var endereco2 = EnderecoEntrega.Criar("12345-678", "Rua A", "Numero 100", "Apto 1", "Bairro B", "SP", "Cidade C", "Brasil");

            
            endereco1.Should().Be(endereco2);
            (endereco1 == endereco2).Should().BeTrue();
        }

        [Fact(DisplayName = "EnderecosEntrega com dados diferentes não devem ser iguais")]
        public void EnderecosDevemSerDiferentes_QuandoPossuemValoresDiferentes()
        {
            
            var endereco1 = EnderecoEntrega.Criar("12345-678", "Rua A", "Numero 100", "Apto 1", "Bairro B", "SP", "Cidade C", "Brasil");
            var endereco2 = EnderecoEntrega.Criar("12345-678", "Rua X", "Numero 100", "Apto 1", "Bairro B", "SP", "Cidade C", "Brasil");
            
            endereco1.Should().NotBe(endereco2);
            (endereco1 != endereco2).Should().BeTrue();
        }

        [Fact(DisplayName = "EnderecoEntrega deve ser imutável após criação")]
        public void EnderecoEntrega_DeveSerImutavel_AposCriacao()
        {
            // Arrange
            var endereco = EnderecoEntrega.Criar("12345-678", "Rua das Flores", "Numero 100", "Apto 101", "Centro", "SP", "São Paulo", "Brasil");

            
            Action act = () =>
            {
                //Tentativa hipotética (conceitual) de alterar uma propriedade
            };

            
            endereco.GetType().GetProperties()
                .All(p => p.SetMethod == null || p.SetMethod.IsPrivate)
                .Should().BeTrue("as propriedades do VO devem ser imutáveis");
        }

        [Theory(DisplayName = "Deve lançar DomainException quando campos obrigatórios forem nulos ou vazios")]
        [InlineData(null, "Rua A", "Numero 100", "Apto 1", "Bairro B", "SP", "Cidade C", "Brasil")]
        [InlineData("12345-678", "", "Numero 100", "Apto 1", "Bairro B", "SP", "Cidade C", "Brasil")]
        [InlineData("12345-678", "Rua A", "Numero 100", "Apto 1", null, "SP", "Cidade C", "Brasil")]

        public void Criar_DeveLancarDomainException_QuandoCamposObrigatoriosForemNulosOuVazios(
            string cep, string logradouro, string numero, string complemento, string bairro,
            string estado, string cidade, string pais)
        {
            // Act
            Action act = () => EnderecoEntrega.Criar(cep, logradouro, numero, complemento, bairro, estado, cidade, pais);

            // Assert
            act.Should().Throw<DomainException>()
                .WithMessage("*não pode ser nulo ou vazio*");
        }
    }
}
