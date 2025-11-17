using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Gs.Dtos.Request;
using Gs.Dtos.Response;
using Gs.Models;
using Gs.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gs.Controllers
{
    /// <summary>
    /// Endpoints para gerenciamento de vagas (Jobs) na plataforma SkillBridge.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")] // 🔹 Versionamento v1
    [Produces("application/json")]
    [Authorize] // 🔐 exige JWT para acessar as vagas
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly ILogger<JobController> _logger;

        public JobController(IJobService jobService, ILogger<JobController> logger)
        {
            _jobService = jobService;
            _logger = logger;
        }

        /// <summary>
        /// Obtém uma lista de vagas com paginação e links HATEOAS.
        /// </summary>
        /// <param name="page">Número da página (inicia em 1).</param>
        /// <param name="pageSize">Quantidade de registros por página.</param>
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var traceId = HttpContext.TraceIdentifier;

            var jobs = await _jobService.GetAllAsync();
            var total = jobs.Count();

            var pagedJobs = jobs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var items = pagedJobs.Select(j => new
            {
                job = MapToResponseDto(j),
                links = new[]
                {
                    new { rel = "self",   href = Url.Action(nameof(GetById), new { id = j.Id }) },
                    new { rel = "update", href = Url.Action(nameof(Update),  new { id = j.Id }) },
                    new { rel = "delete", href = Url.Action(nameof(Delete),  new { id = j.Id }) }
                }
            });

            var response = new
            {
                total,
                page,
                pageSize,
                traceId,   // 🔹 ajuda na correlação de logs
                items
            };

            _logger.LogInformation(
                "Listando vagas - page {Page}, pageSize {PageSize}, traceId {TraceId}",
                page, pageSize, traceId
            );

            return Ok(response);
        }

        /// <summary>
        /// Retorna uma vaga específica por ID.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(JobResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<JobResponseDTO>> GetById(int id)
        {
            var traceId = HttpContext.TraceIdentifier;

            var job = await _jobService.GetByIdAsync(id);
            if (job == null)
            {
                _logger.LogWarning("Vaga {JobId} não encontrada | traceId {TraceId}", id, traceId);
                return NotFound(new { message = "Vaga não encontrada", traceId });
            }

            var response = MapToResponseDto(job);

            _logger.LogInformation("Vaga {JobId} retornada com sucesso | traceId {TraceId}", id, traceId);

            return Ok(response);
        }

        /// <summary>
        /// Cria uma nova vaga.
        /// </summary>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(JobResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<JobResponseDTO>> Create([FromBody] JobRequestDTO jobDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var traceId = HttpContext.TraceIdentifier;

            var job = await _jobService.CreateAsync(jobDto);
            if (job == null)
            {
                _logger.LogError("Falha ao criar vaga | traceId {TraceId}", traceId);
                return BadRequest(new { message = "Não foi possível criar a vaga", traceId });
            }

            var response = MapToResponseDto(job);

            _logger.LogInformation("Vaga criada com Id {JobId} | traceId {TraceId}", response.Id, traceId);

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        /// <summary>
        /// Atualiza uma vaga existente.
        /// </summary>
        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(JobResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<JobResponseDTO>> Update(int id, [FromBody] JobRequestDTO jobDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var traceId = HttpContext.TraceIdentifier;

            var job = await _jobService.UpdateAsync(id, jobDto);
            if (job == null)
            {
                _logger.LogWarning("Tentativa de atualizar vaga inexistente {JobId} | traceId {TraceId}", id, traceId);
                return NotFound(new { message = "Vaga não encontrada", traceId });
            }

            var response = MapToResponseDto(job);

            _logger.LogInformation("Vaga atualizada Id {JobId} | traceId {TraceId}", response.Id, traceId);

            return Ok(response);
        }

        /// <summary>
        /// Remove uma vaga pelo ID.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var traceId = HttpContext.TraceIdentifier;

            var deleted = await _jobService.DeleteAsync(id);
            if (!deleted)
            {
                _logger.LogWarning("Tentativa de deletar vaga inexistente {JobId} | traceId {TraceId}", id, traceId);
                return NotFound(new { message = "Vaga não encontrada", traceId });
            }

            _logger.LogInformation("Vaga removida Id {JobId} | traceId {TraceId}", id, traceId);

            return NoContent();
        }

        /// <summary>
        /// Converte a entidade Job para o DTO de resposta.
        /// </summary>
        private static JobResponseDTO MapToResponseDto(Job job)
        {
            return new JobResponseDTO
            {
                Id = job.Id,
                Titulo = job.Titulo,
                Requisitos = job.Requisitos,
                Empresa = job.Empresa
            };
        }
    }
}
