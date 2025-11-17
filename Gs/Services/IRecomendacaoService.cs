using Gs.Models;
using System.Collections.Generic;

namespace Gs.Services
{
    public interface IRecomendacaoService
    {
        List<JobRecomendacao> RecomendarJobs(Cliente cliente, List<Job> jobs, int topN = 5);
    }
}
