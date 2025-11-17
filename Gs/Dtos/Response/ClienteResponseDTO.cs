using System.ComponentModel.DataAnnotations;

namespace Gs.Dtos.Response
{
    /// <summary>
    /// DTO de saída para retorno de dados de clientes.
    /// </summary>
    /// <remarks>
    /// Usado pelas respostas dos endpoints (ex.: GET /api/Cliente, POST /api/Cliente).
    /// Não expõe a senha do cliente, apenas informações seguras para o consumidor da API.
    /// </remarks>
    public class ClienteResponseDTO
    {
        /// <summary>
        /// Identificador único do cliente.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome completo do cliente.
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// E-mail de contato do cliente.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Profissão atual do cliente.
        /// </summary>
        public string ProfissaoAtual { get; set; }

        /// <summary>
        /// Competências principais do cliente (skills).
        /// </summary>
        public string Competencias { get; set; }
    }
}
