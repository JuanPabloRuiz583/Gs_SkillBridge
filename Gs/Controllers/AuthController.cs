using Gs.Dtos.Request;
using Gs.Dtos.Response;
using Gs.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gs.Controllers
{
    /// <summary>
    /// Endpoints de autenticação (login) da plataforma SkillBridge.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IClienteService clienteService,
            ITokenService tokenService,
            ILogger<AuthController> logger)
        {
            _clienteService = clienteService;
            _tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// Realiza o login do cliente e retorna um token JWT.
        /// </summary>
        /// <param name="loginDto">Credenciais de login (email e senha).</param>
        /// <remarks>
        /// Exemplo de requisição:
        ///
        ///     POST /api/v1/Auth/login
        ///     {
        ///         "email": "joao@email.com",
        ///         "senha": "senha1234"
        ///     }
        ///
        /// Exemplo de resposta (200):
        /// {
        ///   "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///   "cliente": {
        ///     "id": 1,
        ///     "nome": "João da Silva",
        ///     "email": "joao@email.com",
        ///     "profissaoAtual": "Desenvolvedor .NET",
        ///     "competencias": "C#, .NET, SQL"
        ///   }
        /// }
        /// </remarks>
        /// <returns>Token JWT e dados do cliente autenticado.</returns>
        /// <response code="200">Login realizado com sucesso.</response>
        /// <response code="400">Erro de validação no payload.</response>
        /// <response code="401">Email ou senha inválidos.</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<LoginResponseDTO> Login([FromBody] LoginRequestDTO loginDto)
        {
            var traceId = HttpContext.TraceIdentifier;

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "Login inválido - payload com erro de validação | traceId {TraceId}",
                    traceId
                );

                return BadRequest(ModelState);
            }

            var cliente = _clienteService.Authenticate(loginDto.Email, loginDto.Senha);
            if (cliente == null)
            {
                _logger.LogWarning(
                    "Tentativa de login falhou para o email {Email} | traceId {TraceId}",
                    loginDto.Email,
                    traceId
                );

                return Unauthorized(new
                {
                    message = "Email ou senha inválidos",
                    traceId
                });
            }

            // 🔹 Geração centralizada do token via TokenService
            var token = _tokenService.GenerateToken(cliente);

            var response = new LoginResponseDTO
            {
                Token = token,
                Cliente = new ClienteResponseDTO
                {
                    Id = cliente.Id,
                    Nome = cliente.Nome,
                    Email = cliente.Email,
                    ProfissaoAtual = cliente.ProfissaoAtual,
                    Competencias = cliente.Competencias
                }
            };

            _logger.LogInformation(
                "Login realizado com sucesso para o cliente Id {ClienteId} | traceId {TraceId}",
                cliente.Id,
                traceId
            );

            return Ok(response);
        }
    }
}
