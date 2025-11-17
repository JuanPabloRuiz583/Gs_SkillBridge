using Xunit;
using Microsoft.EntityFrameworkCore;
using Gs.Services;
using Gs.Data;
using Gs.Models;
using Gs.Dtos.Request;
using System.Threading.Tasks;
using System.Linq;

namespace Gs.Tests
{
    public class JobServiceTests
    {
        private AppDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        private JobRequestDTO GetValidJobDto(string titulo = "Dev .NET")
        {
            return new JobRequestDTO
            {
                Titulo = titulo,
                Requisitos = "C#, .NET, SQL",
                Empresa = "SkillBridge"
            };
        }

        [Fact]
        public async Task CreateAsync_DeveCriarJob_QuandoDadosValidos()
        {
            var context = GetDbContext("CreateJob");
            var service = new JobService(context);

            var job = await service.CreateAsync(GetValidJobDto());

            Assert.NotNull(job);
            Assert.Equal("Dev .NET", job.Titulo);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarTodosJobs()
        {
            var context = GetDbContext("GetAllJobs");
            var service = new JobService(context);

            await service.CreateAsync(GetValidJobDto("Dev 1"));
            await service.CreateAsync(GetValidJobDto("Dev 2"));

            var jobs = await service.GetAllAsync();

            Assert.Equal(2, jobs.Count());
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarJob_QuandoIdExiste()
        {
            var context = GetDbContext("GetByIdJob");
            var service = new JobService(context);

            var job = await service.CreateAsync(GetValidJobDto());
            var encontrado = await service.GetByIdAsync(job.Id);

            Assert.NotNull(encontrado);
            Assert.Equal(job.Id, encontrado.Id);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarNull_QuandoIdNaoExiste()
        {
            var context = GetDbContext("GetByIdJobNull");
            var service = new JobService(context);

            var job = await service.GetByIdAsync(999);

            Assert.Null(job);
        }

        [Fact]
        public async Task UpdateAsync_DeveAtualizarJob_QuandoIdExiste()
        {
            var context = GetDbContext("UpdateJob");
            var service = new JobService(context);

            var job = await service.CreateAsync(GetValidJobDto());
            var dto = GetValidJobDto("Dev Sênior");
            dto.Requisitos = "C#, .NET, SQL, Azure";
            dto.Empresa = "SkillBridge Pro";

            var atualizado = await service.UpdateAsync(job.Id, dto);

            Assert.NotNull(atualizado);
            Assert.Equal("Dev Sênior", atualizado.Titulo);
            Assert.Equal("SkillBridge Pro", atualizado.Empresa);
        }

        [Fact]
        public async Task UpdateAsync_DeveRetornarNull_QuandoIdNaoExiste()
        {
            var context = GetDbContext("UpdateJobNull");
            var service = new JobService(context);

            var dto = GetValidJobDto("Dev Sênior");
            var atualizado = await service.UpdateAsync(999, dto);

            Assert.Null(atualizado);
        }

        [Fact]
        public async Task DeleteAsync_DeveRemoverJob_QuandoIdExiste()
        {
            var context = GetDbContext("DeleteJob");
            var service = new JobService(context);

            var job = await service.CreateAsync(GetValidJobDto());
            var resultado = await service.DeleteAsync(job.Id);

            Assert.True(resultado);
            var encontrado = await service.GetByIdAsync(job.Id);
            Assert.Null(encontrado);
        }

        [Fact]
        public async Task DeleteAsync_DeveRetornarFalse_QuandoIdNaoExiste()
        {
            var context = GetDbContext("DeleteJobNull");
            var service = new JobService(context);

            var resultado = await service.DeleteAsync(999);

            Assert.False(resultado);
        }
    }
}
