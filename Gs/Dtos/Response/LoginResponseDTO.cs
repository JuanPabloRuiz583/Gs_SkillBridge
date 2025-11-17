namespace Gs.Dtos.Response
{
    /// <summary>
    /// DTO de saída para o endpoint de login.
    /// </summary>
    /// <remarks>
    /// Retorna o token JWT gerado e os dados do cliente autenticado.
    /// </remarks>
    public class LoginResponseDTO
    {
        /// <summary>
        /// Token JWT que deve ser usado no header Authorization (Bearer {token}).
        /// </summary>
        /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</example>
        public string Token { get; set; }

        /// <summary>
        /// Dados do cliente autenticado.
        /// </summary>
        public ClienteResponseDTO Cliente { get; set; }
    }
}
