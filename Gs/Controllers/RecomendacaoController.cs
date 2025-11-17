using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Gs.Models;
using Gs.Services;
using Gs.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gs.Controllers
{
    /// <summary>
    /// Endpoints para recomendação de vagas para clientes.
    /// </summary>
    [ApiController]
    [Route("api/v1/recomendacao")]
    [Produces("application/json")]
    [Authorize] // 🔐 exige JWT
    public class RecomendacaoController : ControllerBase
    {
        private readonly IRecomendacaoService _recomendacaoService;
        private readonly AppDbContext _context;
        private readonly ILogger<RecomendacaoController> _logger;

        public RecomendacaoController(
            IRecomendacaoService recomendacaoService,
            AppDbContext context,
            ILogger<RecomendacaoController> logger)
        {
            _recomendacaoService = recomendacaoService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retorna as vagas mais relevantes para o cliente informado.
        /// </summary>
        /// <param name="clienteId">Id do cliente</param>
        /// <param name="topN">Quantidade máxima de vagas recomendadas</param>
        [HttpGet("jobs/{clienteId:int}")]
        [ProducesResponseType(typeof(IEnumerable<object>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetJobsRecomendados(int clienteId, [FromQuery] int topN = 5)
        {
            var traceId = HttpContext.TraceIdentifier;

            var cliente = await _context.Clientes.FindAsync(clienteId);
            if (cliente == null)
            {
                _logger.LogWarning("Cliente {ClienteId} não encontrado | traceId {TraceId}", clienteId, traceId);
                return NotFound(new { message = "Cliente não encontrado", traceId });
            }

            var jobs = _context.Jobs.ToList();
            if (!jobs.Any())
            {
                _logger.LogWarning("Nenhuma vaga cadastrada | traceId {TraceId}", traceId);
                return NotFound(new { message = "Nenhuma vaga cadastrada", traceId });
            }

            var recomendacoes = _recomendacaoService.RecomendarJobs(cliente, jobs, topN);

            var response = recomendacoes.Select(r => new
            {
                jobId = r.Job.Id,
                titulo = r.Job.Titulo,
                empresa = r.Job.Empresa,
                requisitos = r.Job.Requisitos,
                similaridade = Math.Round(r.Similaridade * 100, 1), // porcentagem com 1 casa decimal
                match = r.Similaridade >= 0.5 ? "forte"
          : r.Similaridade >= 0.3 ? "médio"
          : "baixo",
                matchEmoji = r.Similaridade >= 0.5 ? "🔥"
               : r.Similaridade >= 0.3 ? "👍"
               : "⚠️"
            });



            _logger.LogInformation("Recomendação de vagas para cliente {ClienteId} | traceId {TraceId}", clienteId, traceId);

            return Ok(new
            {
                clienteId,
                total = response.Count(),
                traceId,
                recomendacoes = response
            });
        }
    }
}

