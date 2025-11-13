using Gs.Dtos;

namespace Gs.Services
{
    public interface IJobService
    {
        Task<IEnumerable<JobDTO>> GetAllAsync();
        Task<JobDTO> GetByIdAsync(int id);
        Task<JobDTO> CreateAsync(JobDTO dto);
        Task<JobDTO> UpdateAsync(int id, JobDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
