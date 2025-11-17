using Xunit;
using Microsoft.EntityFrameworkCore;
using Gs.Services;
using Gs.Data;
using Gs.Models;
using Gs.Dtos.Request;
using System.Linq;

namespace Gs.Tests
{

    public class ClienteServiceTests
    {
        private AppDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        private ClienteRequestDTO GetValidClienteDto(string email = "teste@email.com")
        {
            return new ClienteRequestDTO
            {
                Nome = "Teste",
                Email = email,
                Senha = "123456",
                ProfissaoAtual = "Dev",
                Competencias = "C#, .NET"
            };
        }

        [Fact]
        public void Create_DeveCriarCliente_QuandoDadosValidos()
        {
            var context = GetDbContext("CreateCliente");
            var service = new ClienteService(context);

            var (cliente, error) = service.Create(GetValidClienteDto());

            Assert.NotNull(cliente);
            Assert.Null(error);
            Assert.Equal("teste@email.com", cliente.Email);
        }

        [Fact]
        public void Create_DeveRetornarErro_QuandoEmailDuplicado()
        {
            var context = GetDbContext("EmailDuplicado");
            var service = new ClienteService(context);

            service.Create(GetValidClienteDto());
            var (cliente, error) = service.Create(GetValidClienteDto());

            Assert.Null(cliente);
            Assert.Equal("um cliente com esse email ja existe", error);
        }

        [Fact]
        public void GetAll_DeveRetornarTodosClientes()
        {
            var context = GetDbContext("GetAll");
            var service = new ClienteService(context);

            service.Create(GetValidClienteDto("a@email.com"));
            service.Create(GetValidClienteDto("b@email.com"));

            var clientes = service.GetAll().ToList();

            Assert.Equal(2, clientes.Count);
        }


        [Fact]
        public void Update_DeveAtualizarCliente_QuandoDadosValidos()
        {
            var context = GetDbContext("UpdateCliente");
            var service = new ClienteService(context);

            var (cliente, _) = service.Create(GetValidClienteDto());
            var dto = GetValidClienteDto("novo@email.com");
            dto.Nome = "Novo Nome";

            var (atualizado, error) = service.Update(cliente.Id, dto);

            Assert.NotNull(atualizado);
            Assert.Null(error);
            Assert.Equal("Novo Nome", atualizado.Nome);
            Assert.Equal("novo@email.com", atualizado.Email);
        }

        [Fact]
        public void Update_DeveRetornarErro_QuandoEmailJaExisteEmOutroCliente()
        {
            var context = GetDbContext("UpdateEmailDuplicado");
            var service = new ClienteService(context);

            var (cliente1, _) = service.Create(GetValidClienteDto("a@email.com"));
            var (cliente2, _) = service.Create(GetValidClienteDto("b@email.com"));

            var dto = GetValidClienteDto("a@email.com");
            var (atualizado, error) = service.Update(cliente2.Id, dto);

            Assert.Null(atualizado);
            Assert.Equal("Já existe outro cliente com esse e-mail", error);
        }


        [Fact]
        public void Delete_DeveRetornarFalse_QuandoIdNaoExiste()
        {
            var context = GetDbContext("DeleteInexistente");
            var service = new ClienteService(context);

            var resultado = service.Delete(999);

            Assert.False(resultado);
        }

        [Fact]
        public void Authenticate_DeveRetornarCliente_QuandoCredenciaisCorretas()
        {
            var context = GetDbContext("AuthCorreto");
            var service = new ClienteService(context);

            var dto = GetValidClienteDto();
            service.Create(dto);

            var cliente = service.Authenticate(dto.Email, dto.Senha);

            Assert.NotNull(cliente);
            Assert.Equal(dto.Email.ToLowerInvariant(), cliente.Email);
        }

        [Fact]
        public void Authenticate_DeveRetornarNull_QuandoCredenciaisErradas()
        {
            var context = GetDbContext("AuthErrado");
            var service = new ClienteService(context);

            var dto = GetValidClienteDto();
            service.Create(dto);

            var cliente = service.Authenticate(dto.Email, "senhaerrada");

            Assert.Null(cliente);
        }
    }
}