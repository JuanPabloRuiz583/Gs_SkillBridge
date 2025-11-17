using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gs.Controllers
{
    /// <summary>
    /// Endpoint simples de verificação de saúde da API SkillBridge.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    [AllowAnonymous] // 🔓 health não exige JWT (bom pra monitoramento externo)
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Retorna o status básico da API.
        /// </summary>
        /// <remarks>
        /// Endpoints relacionados:
        /// - <b>GET /api/v1/Health</b> → resposta simples JSON (este endpoint)
        /// - <b>GET /health</b>        → health check detalhado configurado no Program.cs
        /// - <b>GET /health-ui</b>     → interface gráfica dos health checks
        ///
        /// Exemplo de requisição:
        ///
        ///     GET /api/v1/Health
        ///
        /// Exemplo de resposta:
        ///
        ///     {
        ///         "status": "Healthy",
        ///         "timestamp": "2025-11-16T21:15:30Z"
        ///     }
        /// </remarks>
        /// <returns>Objeto com status e timestamp em UTC.</returns>
        /// <response code="200">API está saudável.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
