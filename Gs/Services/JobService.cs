using Microsoft.EntityFrameworkCore;
using Gs.Dtos;
using Gs.Models;
using System;
using Gs.Models;
using Gs.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gs.Services
{
    public class JobService : IJobService
    {
        private readonly AppDbContext _context;

        public JobService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<JobDTO>> GetAllAsync()
        {
            return await _context.Jobs
                .Select(j => new JobDTO
                {
                    Id = j.Id,
                    Titulo = j.Titulo,
                    Requisitos = j.Requisitos,
                    Empresa = j.Empresa
                })
            .ToListAsync();
        }

        public async Task<JobDTO> GetByIdAsync(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return null;

            return new JobDTO
            {
                Id = job.Id,
                Titulo = job.Titulo,
                Requisitos = job.Requisitos,
                Empresa = job.Empresa
            };
        }

        public async Task<JobDTO> CreateAsync(JobDTO dto)
        {
            var job = new Job
            {
                Titulo = dto.Titulo,
                Requisitos = dto.Requisitos,
                Empresa = dto.Empresa
            };

            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();

            return new JobDTO
            {
                Id = job.Id,
                Titulo = job.Titulo,
                Requisitos = job.Requisitos,
                Empresa = job.Empresa
            };
        }

        public async Task<JobDTO> UpdateAsync(int id, JobDTO dto)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return null;

            job.Titulo = dto.Titulo;
            job.Requisitos = dto.Requisitos;
            job.Empresa = dto.Empresa;

            _context.Jobs.Update(job);
            await _context.SaveChangesAsync();

            return new JobDTO
            {
                Id = job.Id,
                Titulo = job.Titulo,
                Requisitos = job.Requisitos,
                Empresa = job.Empresa
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return false;

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
