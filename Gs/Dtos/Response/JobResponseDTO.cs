using System.ComponentModel.DataAnnotations;

namespace Gs.Dtos.Response
{
    /// <summary>
    /// DTO de saída para retorno de dados de vagas (Jobs).
    /// </summary>
    /// <remarks>
    /// Usado nas respostas dos endpoints (ex.: GET /api/Job, POST /api/Job).
    /// Inclui o <c>Id</c> gerado pelo sistema.
    /// </remarks>
    public class JobResponseDTO
    {
        /// <summary>
        /// Identificador único da vaga.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Título da vaga.
        /// </summary>
        public string Titulo { get; set; }

        /// <summary>
        /// Requisitos da vaga.
        /// </summary>
        public string Requisitos { get; set; }

        /// <summary>
        /// Nome da empresa contratante.
        /// </summary>
        public string Empresa { get; set; }
    }
}
