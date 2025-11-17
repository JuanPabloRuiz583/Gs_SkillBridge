using System.ComponentModel.DataAnnotations;

namespace Gs.Dtos.Request
{
    /// <summary>
    /// DTO de entrada para criação/atualização de clientes.
    /// </summary>
    /// <remarks>
    /// Usado pelos endpoints de entrada (ex.: POST /api/Cliente, PUT /api/Cliente/{id}).
    /// Não expõe o <c>Id</c>, pois ele é gerenciado pela aplicação/banco de dados.
    /// </remarks>
    public class ClienteRequestDTO
    {
        /// <summary>
        /// Nome completo do cliente.
        /// </summary>
        [Required(ErrorMessage = "Nome não pode estar em branco.")]
        [MaxLength(100, ErrorMessage = "Nome não pode exceder 100 caracteres.")]
        public string Nome { get; set; }

        /// <summary>
        /// E-mail de contato do cliente.
        /// </summary>
        [Required(ErrorMessage = "E-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; }

        /// <summary>
        /// Senha de acesso do cliente.
        /// </summary>
        /// <remarks>
        /// A senha deve ter no mínimo 6 caracteres.
        /// No banco, recomenda-se armazenar a senha de forma criptografada (hash).
        /// </remarks>
        [Required(ErrorMessage = "Senha não pode estar em branco.")]
        [MinLength(6, ErrorMessage = "Senha deve ter pelo menos 6 caracteres.")]
        public string Senha { get; set; }

        /// <summary>
        /// Profissão atual do cliente.
        /// </summary>
        [MaxLength(100, ErrorMessage = "Profissão não pode exceder 100 caracteres.")]
        public string ProfissaoAtual { get; set; }

        /// <summary>
        /// Competências principais do cliente (skills).
        /// </summary>
        [Required(ErrorMessage = "As competências são obrigatórias.")]
        [MaxLength(300, ErrorMessage = "Competências não podem exceder 300 caracteres.")]
        public string Competencias { get; set; }
    }
}
