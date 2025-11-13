using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Gs.Dtos;
using Gs.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Gs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔒 todos os endpoints exigem JWT válido
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly LinkGenerator _linkGenerator;

        public JobController(IJobService jobService, LinkGenerator linkGenerator)
        {
            _jobService = jobService;
            _linkGenerator = linkGenerator;
        }

        /// <summary>
        /// Lista todas as vagas com paginação.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<object>))]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var jobs = await _jobService.GetAllAsync();

            var total = jobs.Count();
            var pagedJobs = jobs
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var result = pagedJobs.Select(j => new
            {
                job = j,
                links = new[]
                {
                    new { rel = "self",   href = _linkGenerator.GetPathByAction("GetById", "Job", new { id = j.Id }) },
                    new { rel = "update", href = _linkGenerator.GetPathByAction("Update", "Job", new { id = j.Id }) },
                    new { rel = "delete", href = _linkGenerator.GetPathByAction("Delete", "Job", new { id = j.Id }) }
                }
            });

            var response = new
            {
                total,
                page,
                pageSize,
                items = result
            };

            return Ok(response);
        }

        /// <summary>
        /// Busca uma vaga pelo ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var job = await _jobService.GetByIdAsync(id);
            if (job == null)
                return NotFound();

            var result = new
            {
                job,
                links = new[]
                {
                    new { rel = "self",   href = _linkGenerator.GetPathByAction("GetById", "Job", new { id }) },
                    new { rel = "update", href = _linkGenerator.GetPathByAction("Update", "Job", new { id }) },
                    new { rel = "delete", href = _linkGenerator.GetPathByAction("Delete", "Job", new { id }) }
                }
            };

            return Ok(result);
        }

        /// <summary>
        /// Cria uma nova vaga.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] JobDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var job = await _jobService.CreateAsync(dto);
            var location = _linkGenerator.GetPathByAction("GetById", "Job", new { id = job.Id });

            var result = new
            {
                job,
                links = new[]
                {
                    new { rel = "self",   href = location },
                    new { rel = "update", href = _linkGenerator.GetPathByAction("Update", "Job", new { id = job.Id }) },
                    new { rel = "delete", href = _linkGenerator.GetPathByAction("Delete", "Job", new { id = job.Id }) }
                }
            };

            return Created(location!, result);
        }

        /// <summary>
        /// Atualiza uma vaga existente.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] JobDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var job = await _jobService.UpdateAsync(id, dto);
            if (job == null)
                return NotFound();

            var result = new
            {
                job,
                links = new[]
                {
                    new { rel = "self",   href = _linkGenerator.GetPathByAction("GetById", "Job", new { id }) },
                    new { rel = "update", href = _linkGenerator.GetPathByAction("Update", "Job", new { id }) },
                    new { rel = "delete", href = _linkGenerator.GetPathByAction("Delete", "Job", new { id }) }
                }
            };

            return Ok(result);
        }

        /// <summary>
        /// Exclui uma vaga pelo ID.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _jobService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
