using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Gs.Dtos;
using Gs.Models;
using Gs.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Gs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly IConfiguration _configuration;

        public ClienteController(IClienteService clienteService,
                                 IConfiguration configuration)
        {
            _clienteService = clienteService;
            _configuration = configuration;
        }

        /// <summary>
        /// Obtém uma lista de todos os clientes.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Cliente>))]
        public IActionResult GetAll()
        {
            var clientes = _clienteService.GetAll();
            return Ok(clientes);
        }

        /// <summary>
        /// Retorna um cliente específico por ID.
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Cliente))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(long id)
        {
            var cliente = _clienteService.GetById(id);
            if (cliente == null) return NotFound();
            return Ok(cliente);
        }

        /// <summary>
        /// Cria um novo cliente.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Cliente))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult Create([FromBody] ClienteDTO clienteDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (cliente, error) = _clienteService.Create(clienteDto);
            if (error == "um cliente com esse email ja existe")
                return Conflict(new { message = error });

            if (cliente == null)
                return BadRequest();

            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }

        /// <summary>
        /// Remove um cliente pelo ID.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(long id)
        {
            var deleted = _clienteService.Delete(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Realiza o login do cliente e retorna um token JWT.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous] // login liberado mesmo se você colocar [Authorize] na classe depois
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cliente = _clienteService.Authenticate(loginDto.Email, loginDto.Senha);
            if (cliente == null)
                return Unauthorized(new { message = "Email ou senha inválidos" });

            var tokenHandler = new JwtSecurityTokenHandler();

            var keyString = _configuration["Jwt:Key"]
                            ?? throw new InvalidOperationException("Jwt:Key não configurado no appsettings.json");

            var expireMinutesString = _configuration["Jwt:ExpireMinutes"];
            var expireMinutes = int.TryParse(expireMinutesString, out var minutes) ? minutes : 60;

            var key = Encoding.ASCII.GetBytes(keyString);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, cliente.Id.ToString()),
                new Claim(ClaimTypes.Email, cliente.Email),
                new Claim(ClaimTypes.Name, cliente.Nome)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                cliente = new { cliente.Id, cliente.Nome, cliente.Email }
            });
        }
    }
}
