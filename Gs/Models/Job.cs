using System.ComponentModel.DataAnnotations;

namespace Gs.Models
{
    /// <summary>
    /// Representa uma vaga de emprego cadastrada na plataforma SkillBridge.
    /// </summary>
    public class Job
    {
        /// <summary>
        /// Identificador único da vaga (chave primária).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Título da vaga (ex.: Desenvolvedor .NET Junior).
        /// </summary>
        [Required(ErrorMessage = "Título da vaga é obrigatório")]
        [MaxLength(120, ErrorMessage = "Título não pode exceder 120 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Requisitos principais da vaga (skills, experiências, etc.).
        /// </summary>
        [Required(ErrorMessage = "Requisitos são obrigatórios")]
        [MaxLength(300, ErrorMessage = "Requisitos não podem exceder 300 caracteres")]
        public string Requisitos { get; set; } = string.Empty;

        /// <summary>
        /// Nome da empresa que está oferecendo a vaga.
        /// </summary>
        [Required(ErrorMessage = "Nome da empresa é obrigatório")]
        [MaxLength(100, ErrorMessage = "Nome da empresa não pode exceder 100 caracteres")]
        public string Empresa { get; set; } = string.Empty;
    }
}
