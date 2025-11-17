using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Gs.Models;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Gs.Services
{
    /// <summary>
    /// Serviço responsável por gerar tokens JWT para autenticação
    /// dos clientes da aplicação.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Cria uma nova instância do <see cref="TokenService"/>,
        /// utilizando as configurações da aplicação (appsettings).
        /// </summary>
        /// <param name="configuration">
        /// Fonte de configuração, usada para ler as chaves da seção <c>Jwt</c>.
        /// </param>
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Gera um token JWT assinado para o cliente informado.
        /// </summary>
        /// <param name="cliente">
        /// Cliente autenticado para o qual o token será emitido.
        /// As claims básicas (Id, Nome, Email) são extraídas deste objeto.
        /// </param>
        /// <returns>
        /// Uma string representando o token JWT pronto para ser usado no header
        /// <c>Authorization: Bearer {{token}}</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Lançada se o objeto <paramref name="cliente"/> for nulo.
        /// </exception>
        public string GenerateToken(Cliente cliente)
        {
            if (cliente == null)
                throw new ArgumentNullException(nameof(cliente));

            var tokenHandler = new JwtSecurityTokenHandler();

            // 🔐 Chave secreta: preferencialmente vinda do appsettings.json (Jwt:Key).
            // Se não houver configuração, usa a mesma chave padrão do Program.cs.
            var keyString = _configuration["Jwt:Key"]
                            ?? "minha-chave-super-secreta-1234567890";

            // Tempo de expiração em minutos (Jwt:ExpireMinutes). Default: 60.
            var expireMinutesString = _configuration["Jwt:ExpireMinutes"];
            var expireMinutes = int.TryParse(expireMinutesString, out var minutes)
                ? minutes
                : 60;

            var keyBytes = Encoding.ASCII.GetBytes(keyString);

            // Claims básicas do usuário autenticado
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, cliente.Id.ToString()),
                new Claim(ClaimTypes.Email, cliente.Email),
                new Claim(ClaimTypes.Name, cliente.Nome)
                // 👉 futuramente dá pra adicionar Role/Perfil, etc.
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
