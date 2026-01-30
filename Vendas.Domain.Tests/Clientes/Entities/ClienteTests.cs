using FluentAssertions;
using Vendas.Domain.Clientes.Entities;
using Vendas.Domain.Clientes.Enums;
using Vendas.Domain.Clientes.Events;
using Vendas.Domain.Clientes.ValueObjects;
using Vendas.Domain.Common.Exceptions;

namespace Vendas.Domain.Tests.Clientes.Entities
{
    public class ClienteTests()
    {
        private static NomeCompleto CriarNomeCompleto(string nome = "João Silva") => new(nome);

        private static Cpf CriarCpf(string cpf = "12345678909") => new(cpf);

        private static Email CriarEmail(string email = "joao@example.com") => new(email);

        private static Telefone CriarTelefone(string telefone = "11999999999") => new(telefone);

        private static Endereco CriarEndereco(
            string cep = "01310100",
            string logradouro = "Avenida Paulista",
            string numero = "1000",
            string bairro = "Bela Vista",
            string cidade = "São Paulo",
            string estado = "SP",
            string pais = "Brasil",
            string complemento = "")
            => new(cep, logradouro, numero, bairro, cidade, estado, pais, complemento);

        private static Cliente CriarClienteValido() => new Cliente(
            CriarNomeCompleto(),
            CriarCpf(),
            CriarEmail(),
            CriarTelefone(),
            CriarEndereco(),
            Sexo.Masculino,
            EstadoCivil.Solteiro);

        [Fact(DisplayName = "Construtor com dados válidos deve criar cliente.")]
        public void Construtor_ComDadosValidos_DeveCriarCliente()
        {
            //Arrange & Act
            var cliente = CriarClienteValido();

            //Assert
            cliente.Status.Should().Be(StatusCliente.Ativo);
            cliente.Sexo.Should().Be(Sexo.Masculino);
            cliente.EstadoCivil.Should().Be(EstadoCivil.Solteiro);
            cliente.Enderecos.Should().ContainSingle();
            cliente.EnderecoPrincipalId.Should().Be(cliente.Enderecos.First().Id);
        }

        [Fact(DisplayName = "Construtor deve gerar evento cliente cadastrado")]
        public void Construtor_DeveGerarEventoClienteCadastrado()
        {
            //Arrange & Act
            var cliente = CriarClienteValido();

            //Assert
            cliente.DomainEvents.Should().ContainSingle().Which.Should().BeOfType<ClienteCadastradoEvent>();
        }

        [Theory(DisplayName = "Construtor com parâmetro orbigatório nulo deve lançar domain exception")]
        [InlineData("Nome")]
        [InlineData("Cpf")]
        [InlineData("Email")]
        [InlineData("Telefone")]
        [InlineData("Endereco")]
        public void Construtor_ComParametroObrigatorioNulo_DeveLancarDomainException(string campo)
        {
            //Arrange
            NomeCompleto? nome = campo == "Nome" ? null : CriarNomeCompleto();
            Cpf? cpf = campo == "Cpf" ? null : CriarCpf();
            Email? email = campo == "Email" ? null : CriarEmail();
            Telefone? telefone = campo == "Telefone" ? null : CriarTelefone();
            Endereco? endereco = campo == "Endereco" ? null : CriarEndereco();

            //Act
            Action act = () => new Cliente(nome!, cpf!, email!, telefone!, endereco!);

            //Assert
            act.Should().Throw<DomainException>();
        }

        [Fact(DisplayName = "AdicionarEndereco deve adicionar")]
        public void AdicionarEndereco_DeveAdicionar()
        {
            //Arrage
            var cliente = CriarClienteValido();
            var novo = CriarEndereco("02134000", "Rua Augusta");

            //Act
            cliente.AdicionarEndereco(novo);

            //Assert
            cliente.Enderecos.Should().HaveCount(2);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AdicionarEndereco_ValidacaoDeNuLo(bool usarNulo)
        {
            //Arrange
            var cliente = CriarClienteValido();
            Endereco? endereco = usarNulo ? null : CriarEndereco();

            //Act
            Action act = () => cliente.AdicionarEndereco(endereco!);

            //Assert
            if (usarNulo)
                act.Should().Throw<DomainException>();
            else
                act.Should().NotThrow();
        }

        [Fact(DisplayName = "AdicionarEndereco deve atualizar data de modificação")]
        public void AdicionarEndereco_DeveAtualizarDataModificacao()
        {
            //Arrange
            var cliente = CriarClienteValido();
            var dataAnterior = cliente.DataAtualizacao ?? DateTime.UtcNow;

            //Act
            System.Threading.Thread.Sleep(5);
            cliente.AdicionarEndereco(CriarEndereco("02134000", "Rua Augusta"));

            //Assert
            cliente.DataAtualizacao.Should().BeAfter(dataAnterior);
        }

        [Fact]
        public void RemoverEndereco_ComSegundoEndereco_DeveRemover()
        {
            var cliente = CriarClienteValido();
            var segundo = CriarEndereco("02134000", "Rua Augusta");
            cliente.AdicionarEndereco(segundo);

            cliente.RemoverEndereco(segundo.Id);

            cliente.Enderecos.Should().HaveCount(1);
        }

        [Theory]
        [InlineData("NaoExiste")]
        [InlineData("Ultimo")]
        public void RemoverEndereco_DeveLancarExceptions(string caso)
        {
            var cliente = CriarClienteValido();
            Guid id = caso switch
            {
                "NaoExiste" => Guid.NewGuid(),
                "Ultimo" => cliente.EnderecoPrincipalId,
                _ => throw new ArgumentOutOfRangeException()
            };

            Action act = () => cliente.RemoverEndereco(id);

            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void RemoverEndereco_Principal_DeveRedefinirPrincipal()
        {
            var cliente = CriarClienteValido();
            var segundo = CriarEndereco("02134000", "Rua Augusta");
            cliente.AdicionarEndereco(segundo);

            cliente.RemoverEndereco(cliente.EnderecoPrincipalId);

            cliente.EnderecoPrincipalId.Should().Be(segundo.Id);
            cliente.DomainEvents.Should().Contain(e => e is EnderecoPrincipalAlteradoEvent);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AlterarEndereco_ValidacaoDeEndereco(bool valido)
        {
            var cliente = CriarClienteValido();
            var principal = cliente.ObterEnderecoPrincipal();

            Guid id = valido ? principal.Id : Guid.NewGuid();

            Action act = () => cliente.AlterarEndereco(id, "02134000", "Rua Nova", "1", "Centro", "São Paulo", "SP", "Brasil");

            if (valido)
                act.Should().NotThrow();
            else
                act.Should().Throw<DomainException>();
        }

        [Fact]
        public void AlterarEndereco_DeveAlterarCampos()
        {
            var cliente = CriarClienteValido();
            var principal = cliente.ObterEnderecoPrincipal();

            cliente.AlterarEndereco(principal.Id, "02134000", "Rua Nova", "1", "Centro", "São Paulo", "SP", "Brasil");

            principal.Logradouro.Should().Be("Rua Nova");
        }

        [Fact]
        public void DefinirEnderecoPrincipal_DeveDefinir()
        {
            var cliente = CriarClienteValido();
            var novo = CriarEndereco("02134000", "Rua Augusta");
            cliente.AdicionarEndereco(novo);

            cliente.DefinirEnderecoPrincipal(novo.Id);

            cliente.EnderecoPrincipalId.Should().Be(novo.Id);
        }

        [Fact]
        public void DefinirEnderecoPrincipal_DeveGerarEvento()
        {
            var cliente = CriarClienteValido();
            var novo = CriarEndereco("02134000", "Rua Augusta");
            cliente.AdicionarEndereco(novo);

            cliente.DefinirEnderecoPrincipal(novo.Id);

            cliente.DomainEvents.Should().Contain(e => e is EnderecoPrincipalAlteradoEvent);
        }

        [Fact]
        public void ObterEnderecoPrincipal_DeveRetornarCorreto()
        {
            var cliente = CriarClienteValido();

            var principal = cliente.ObterEnderecoPrincipal();

            principal.Id.Should().Be(cliente.EnderecoPrincipalId);
        }

        [Fact]
        public void AtualizarPerfil_DeveAtualizar()
        {
            var cliente = CriarClienteValido();
            var nome = CriarNomeCompleto("Maria Silva");

            cliente.AtualizarPerfil(
                nome,
                CriarEmail("maria@ex.com"),
                CriarTelefone("11988887777"),
                Sexo.Feminino,
                EstadoCivil.Casado);

            cliente.Nome.Should().Be(nome);
            cliente.Sexo.Should().Be(Sexo.Feminino);
            cliente.EstadoCivil.Should().Be(EstadoCivil.Casado);
        }

        [Theory]
        [InlineData("Nome")]
        [InlineData("Email")]
        [InlineData("Telefone")]
        public void AtualizarPerfil_CamposNulos_DeveFalhar(string campo)
        {
            var cliente = CriarClienteValido();

            NomeCompleto? nome = campo == "Nome" ? null : CriarNomeCompleto();
            Email? email = campo == "Email" ? null : CriarEmail();
            Telefone? telefone = campo == "Telefone" ? null : CriarTelefone();

            Action act = () => cliente.AtualizarPerfil(
                nome!, email!, telefone!,
                Sexo.Masculino,
                EstadoCivil.Solteiro);

            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void AtualizarPerfil_ComClienteBloqueado_DeveFalhar()
        {
            var cliente = CriarClienteValido();
            cliente.Bloquear();

            Action act = () => cliente.AtualizarPerfil(
                CriarNomeCompleto(),
                CriarEmail(),
                CriarTelefone(),
                Sexo.Masculino,
                EstadoCivil.Casado);

            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void Bloquear_DeveBloquear()
        {
            var cliente = CriarClienteValido();

            cliente.Bloquear();

            cliente.Status.Should().Be(StatusCliente.Bloqueado);
        }

        [Fact]
        public void Bloquear_DeveGerarEvento()
        {
            var cliente = CriarClienteValido();

            cliente.Bloquear();

            cliente.DomainEvents.Should().Contain(e => e is ClienteBloqueadoEvent);
        }

        [Fact]
        public void Ativar_DeveAtivar()
        {
            var cliente = CriarClienteValido();
            cliente.Bloquear();

            cliente.Ativar();

            cliente.Status.Should().Be(StatusCliente.Ativo);
        }

        [Fact]
        public void Fluxo_Completo_DeveManterConsistencia()
        {
            var cliente = CriarClienteValido();

            var e1 = CriarEndereco("02134000", "Rua Augusta");
            cliente.AdicionarEndereco(e1);

            cliente.DefinirEnderecoPrincipal(e1.Id);

            cliente.EnderecoPrincipalId.Should().Be(e1.Id);
            cliente.Enderecos.Should().HaveCount(2);
        }
    }
}
