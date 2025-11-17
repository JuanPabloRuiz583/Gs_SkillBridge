using System.Collections.Generic;
using System.Linq;

using Gs.Dtos.Request;
using Gs.Dtos.Response;
using Gs.Models;
using Gs.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gs.Controllers
{
    /// <summary>
    /// Endpoints para gerenciamento de clientes da plataforma SkillBridge.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly ILogger<ClienteController> _logger;

        public ClienteController(
            IClienteService clienteService,
            ILogger<ClienteController> logger)
        {
            _clienteService = clienteService;
            _logger = logger;
        }

        /// <summary>
        /// Obtém uma lista de clientes com paginação e links HATEOAS.
        /// </summary>
        /// <param name="page">Número da página (inicia em 1).</param>
        /// <param name="pageSize">Quantidade de registros por página.</param>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     GET /api/v1/Cliente?page=1&amp;pageSize=10
        ///
        /// </remarks>
        /// <returns>Lista paginada de clientes com links.</returns>
        /// <response code="200">Retorna a lista paginada de clientes.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        public ActionResult<object> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var traceId = HttpContext.TraceIdentifier;

            var clientes = _clienteService.GetAll().ToList();
            var total = clientes.Count;

            var pagedClientes = clientes
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var items = pagedClientes.Select(c => new
            {
                cliente = MapToResponseDto(c),
                links = new[]
                {
                    new { rel = "self",   href = Url.Action(nameof(GetById), new { id = c.Id }) },
                    new { rel = "delete", href = Url.Action(nameof(Delete),  new { id = c.Id }) }
                }
            });

            var response = new
            {
                total,
                page,
                pageSize,
                items,
                traceId
            };

            _logger.LogInformation(
                "Listando clientes - page {Page}, pageSize {PageSize}, total {Total} | traceId {TraceId}",
                page,
                pageSize,
                total,
                traceId
            );

            return Ok(response);
        }

        /// <summary>
        /// Retorna um cliente específico por ID.
        /// </summary>
        /// <param name="id">ID do cliente.</param>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     GET /api/v1/Cliente/1
        ///
        /// </remarks>
        /// <returns>O cliente encontrado ou NotFound.</returns>
        /// <response code="200">Retorna o cliente.</response>
        /// <response code="404">Cliente não encontrado.</response>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClienteResponseDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ClienteResponseDTO> GetById(long id)
        {
            var traceId = HttpContext.TraceIdentifier;

            var cliente = _clienteService.GetById(id);
            if (cliente == null)
            {
                _logger.LogWarning(
                    "Cliente {ClienteId} não encontrado | traceId {TraceId}",
                    id,
                    traceId
                );

                return NotFound(new { message = "Cliente não encontrado", traceId });
            }

            var response = MapToResponseDto(cliente);

            _logger.LogInformation(
                "Cliente {ClienteId} retornado com sucesso | traceId {TraceId}",
                id,
                traceId
            );

            return Ok(response);
        }

        /// <summary>
        /// Cria um novo cliente.
        /// </summary>
        /// <param name="clienteDto">Dados do cliente a ser criado.</param>
        /// <remarks>
        /// O ID do cliente é gerado automaticamente.
        /// Exemplo de requisição:
        ///
        ///     POST /api/v1/Cliente
        ///     {
        ///         "nome": "João da Silva",
        ///         "email": "joao@email.com",
        ///         "senha": "senha1234",
        ///         "profissaoAtual": "Desenvolvedor .NET",
        ///         "competencias": "C#, .NET, SQL"
        ///     }
        /// </remarks>
        /// <returns>O cliente recém-criado, incluindo o ID.</returns>
        /// <response code="201">Retorna o cliente recém-criado.</response>
        /// <response code="400">Se o cliente for nulo ou inválido.</response>
        /// <response code="409">Se já existir um cliente com o mesmo e-mail.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ClienteResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<ClienteResponseDTO> Create([FromBody] ClienteRequestDTO clienteDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var traceId = HttpContext.TraceIdentifier;

            var (cliente, error) = _clienteService.Create(clienteDto);

            if (error == "um cliente com esse email ja existe")
            {
                _logger.LogWarning(
                    "Tentativa de criar cliente com e-mail duplicado {Email} | traceId {TraceId}",
                    clienteDto.Email,
                    traceId
                );

                return Conflict(new { message = error, traceId });
            }

            if (cliente == null)
            {
                _logger.LogError(
                    "Falha ao criar cliente | traceId {TraceId} | erro: {Erro}",
                    traceId,
                    error
                );

                return BadRequest(new { message = error ?? "Não foi possível criar o cliente", traceId });
            }

            var response = MapToResponseDto(cliente);

            _logger.LogInformation(
                "Cliente criado com Id {ClienteId} | traceId {TraceId}",
                response.Id,
                traceId
            );

            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, response);
        }

        /// <summary>
        /// Remove um cliente pelo ID.
        /// </summary>
        /// <param name="id">ID do cliente a ser removido.</param>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     DELETE /api/v1/Cliente/1
        ///
        /// </remarks>
        /// <response code="204">Cliente removido com sucesso.</response>
        /// <response code="404">Cliente não encontrado.</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(long id)
        {
            var traceId = HttpContext.TraceIdentifier;

            var deleted = _clienteService.Delete(id);
            if (!deleted)
            {
                _logger.LogWarning(
                    "Tentativa de remover cliente inexistente {ClienteId} | traceId {TraceId}",
                    id,
                    traceId
                );

                return NotFound(new { message = "Cliente não encontrado", traceId });
            }

            _logger.LogInformation(
                "Cliente removido Id {ClienteId} | traceId {TraceId}",
                id,
                traceId
            );

            return NoContent();
        }





        /// <summary>
        /// Atualiza os dados de um cliente existente.
        /// </summary>
        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(Cliente), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Update(long id, [FromBody] ClienteRequestDTO clienteDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (cliente, error) = _clienteService.Update(id, clienteDto);
            if (error == "Cliente não encontrado")
                return NotFound(new { message = error });
            if (error != null)
                return BadRequest(new { message = error });

            return Ok(cliente);
        }



        // =========================
        // 🔹 Helper de mapeamento
        // =========================
        private static ClienteResponseDTO MapToResponseDto(Cliente cliente)
        {
            return new ClienteResponseDTO
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Email = cliente.Email,
                ProfissaoAtual = cliente.ProfissaoAtual,
                Competencias = cliente.Competencias
            };
        }
    }
}
