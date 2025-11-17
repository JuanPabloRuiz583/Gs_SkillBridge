using System.Collections.Generic;
using System.Threading.Tasks;

using Gs.Data;
using Gs.Dtos.Request;
using Gs.Models;

using Microsoft.EntityFrameworkCore;

namespace Gs.Services
{
    /// <summary>
    /// Implementação dos serviços de vagas (Jobs).
    /// </summary>
    public class JobService : IJobService
    {
        private readonly AppDbContext _context;

        public JobService(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Job>> GetAllAsync()
        {
            return await _context.Jobs
                .AsNoTracking()
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Job?> GetByIdAsync(int id)
        {
            return await _context.Jobs
                .AsNoTracking()
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        /// <inheritdoc />
        public async Task<Job> CreateAsync(JobRequestDTO dto)
        {
            var job = new Job
            {
                Titulo = dto.Titulo,
                Requisitos = dto.Requisitos,
                Empresa = dto.Empresa
            };

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            return job;
        }

        /// <inheritdoc />
        public async Task<Job?> UpdateAsync(int id, JobRequestDTO dto)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return null;

            job.Titulo = dto.Titulo;
            job.Requisitos = dto.Requisitos;
            job.Empresa = dto.Empresa;

            _context.Jobs.Update(job);
            await _context.SaveChangesAsync();

            return job;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
                return false;

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
