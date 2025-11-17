using System.ComponentModel.DataAnnotations;

namespace Gs.Dtos.Request
{
    /// <summary>
    /// DTO de entrada para autenticação (login) do cliente.
    /// </summary>
    /// <remarks>
    /// Utilizado no endpoint:
    /// 
    ///     POST /api/Auth/login
    /// 
    /// Envie o e-mail e a senha do cliente já cadastrado.
    /// </remarks>
    public class LoginRequestDTO
    {
        /// <summary>
        /// E-mail do cliente.
        /// </summary>
        /// <example>teste@email.com</example>
        [Required(ErrorMessage = "E-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; }

        /// <summary>
        /// Senha do cliente.
        /// </summary>
        /// <example>senha123</example>
        [Required(ErrorMessage = "Senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "Senha deve ter pelo menos 6 caracteres.")]
        public string Senha { get; set; }
    }
}
