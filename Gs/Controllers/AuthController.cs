using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Gs.Dtos;
using Gs.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Gs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly IConfiguration _configuration;

        public AuthController(IClienteService clienteService,
                              IConfiguration configuration)
        {
            _clienteService = clienteService;
            _configuration = configuration;
        }

        /// <summary>
        /// Realiza o login e retorna um token JWT.
        /// </summary>
        /// <param name="loginDto">Credenciais (email e senha).</param>
        /// <returns>Token JWT e dados básicos do cliente.</returns>
        /// <response code="200">Login ok.</response>
        /// <response code="401">Email ou senha inválidos.</response>
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cliente = _clienteService.Authenticate(loginDto.Email, loginDto.Senha);
            if (cliente == null)
                return Unauthorized(new { message = "Email ou senha inválidos" });

            // === Gera o token usando appsettings.json ===
            var tokenHandler = new JwtSecurityTokenHandler();

            var keyString = _configuration["Jwt:Key"]
                            ?? throw new InvalidOperationException("Jwt:Key não configurado");

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
