using Gs.Models;

namespace Gs.Services
{
    /// <summary>
    /// Serviço responsável pela geração de tokens JWT para autenticação.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Gera um token JWT para o cliente informado,
        /// contendo as claims básicas de identificação.
        /// </summary>
        /// <param name="cliente">
        /// Objeto <see cref="Cliente"/> autenticado, do qual serão extraídos
        /// os dados (Id, Nome, Email) para compor o token.
        /// </param>
        /// <returns>
        /// Uma string representando o token JWT assinado, pronto para uso
        /// no header <c>Authorization: Bearer {{token}}</c>.
        /// </returns>
        string GenerateToken(Cliente cliente);
    }
}
