using System.Collections.Generic;
using System.Threading.Tasks;

using Gs.Dtos.Request;
using Gs.Models;

namespace Gs.Services
{
    /// <summary>
    /// Contrato de serviços para gerenciamento de vagas (Jobs).
    /// </summary>
    public interface IJobService
    {
        /// <summary>
        /// Retorna todas as vagas cadastradas.
        /// </summary>
        Task<IEnumerable<Job>> GetAllAsync();

        /// <summary>
        /// Busca uma vaga pelo identificador.
        /// </summary>
        Task<Job?> GetByIdAsync(int id);

        /// <summary>
        /// Cria uma nova vaga com base nos dados de requisição.
        /// </summary>
        Task<Job> CreateAsync(JobRequestDTO dto);

        /// <summary>
        /// Atualiza uma vaga existente.
        /// </summary>
        Task<Job?> UpdateAsync(int id, JobRequestDTO dto);

        /// <summary>
        /// Remove uma vaga pelo identificador.
        /// </summary>
        Task<bool> DeleteAsync(int id);
    }
}
