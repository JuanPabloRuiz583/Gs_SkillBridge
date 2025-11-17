using System.ComponentModel.DataAnnotations;

namespace Gs.Dtos.Request
{
    /// <summary>
    /// DTO de entrada para criação/atualização de vagas (Jobs).
    /// </summary>
    /// <remarks>
    /// Usado nos endpoints de entrada (ex.: POST /api/Job, PUT /api/Job/{id}).
    /// Não expõe o <c>Id</c>, pois ele é controlado pela aplicação/banco.
    /// </remarks>
    public class JobRequestDTO
    {
        /// <summary>
        /// Título da vaga.
        /// </summary>
        /// <example>Desenvolvedor .NET Júnior</example>
        [Required(ErrorMessage = "Título da vaga é obrigatório.")]
        [MaxLength(120, ErrorMessage = "Título não pode exceder 120 caracteres.")]
        public string Titulo { get; set; }

        /// <summary>
        /// Requisitos da vaga (skills, experiência, etc.).
        /// </summary>
        /// <example>Experiência com .NET, SQL Server e APIs REST.</example>
        [Required(ErrorMessage = "Requisitos são obrigatórios.")]
        [MaxLength(300, ErrorMessage = "Requisitos não podem exceder 300 caracteres.")]
        public string Requisitos { get; set; }

        /// <summary>
        /// Nome da empresa contratante.
        /// </summary>
        /// <example>FIAP Tech</example>
        [Required(ErrorMessage = "Nome da empresa é obrigatório.")]
        [MaxLength(100, ErrorMessage = "Nome da empresa não pode exceder 100 caracteres.")]
        public string Empresa { get; set; }
    }
}
