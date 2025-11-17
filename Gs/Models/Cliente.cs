using System.ComponentModel.DataAnnotations;

namespace Gs.Models
{
    /// <summary>
    /// Representa um cliente cadastrado na plataforma SkillBridge.
    /// </summary>
    public class Cliente
    {
        /// <summary>
        /// Identificador único do cliente (chave primária).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome completo do cliente.
        /// </summary>
        [Required(ErrorMessage = "Nome não pode estar em branco")]
        [MaxLength(100, ErrorMessage = "Nome não pode exceder 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Endereço de e-mail do cliente.
        /// </summary>
        [Required(ErrorMessage = "E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
        [MaxLength(200, ErrorMessage = "E-mail não pode exceder 200 caracteres")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Senha em texto simples cadastrada pelo cliente.
        /// </summary>
        /// <remarks>
        /// Em um cenário real de produção, esta propriedade deveria armazenar
        /// apenas o hash da senha, nunca a senha em texto puro.
        /// </remarks>
        [Required(ErrorMessage = "Senha não pode estar em branco")]
        [MinLength(6, ErrorMessage = "Senha deve ter pelo menos 6 caracteres")]
        public string Senha { get; set; } = string.Empty;

        /// <summary>
        /// Profissão atual declarada pelo cliente.
        /// </summary>
        [MaxLength(100, ErrorMessage = "Profissão não pode exceder 100 caracteres")]
        public string? ProfissaoAtual { get; set; }

        /// <summary>
        /// Competências principais do cliente (skills, tecnologias, etc.).
        /// </summary>
        [Required(ErrorMessage = "As competências são obrigatórias")]
        [MaxLength(300, ErrorMessage = "Competências não podem exceder 300 caracteres")]
        public string Competencias { get; set; } = string.Empty;
    }
}
