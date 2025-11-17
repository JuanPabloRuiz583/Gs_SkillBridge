using Microsoft.ML;
using Microsoft.ML.Data;
using Gs.Models;
using System.Collections.Generic;
using System.Linq;

namespace Gs.Services
{
    // Classes auxiliares
    public class JobInput
    {
        public string Requisitos { get; set; }
    }

    public class JobVector
    {
        [VectorType]
        public float[] Features { get; set; }
    }

    public class JobRecomendacao
    {
        public Job Job { get; set; }
        public float Similaridade { get; set; }
    }

    // Serviço de recomendação
    public class RecomendacaoService : IRecomendacaoService
    {
        public List<JobRecomendacao> RecomendarJobs(Cliente cliente, List<Job> jobs, int topN = 5)
        {
            var mlContext = new MLContext();

            var jobInputs = jobs.Select(j => new JobInput { Requisitos = j.Requisitos }).ToList();
            var data = mlContext.Data.LoadFromEnumerable(jobInputs);

            var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(JobInput.Requisitos));
            var model = pipeline.Fit(data);
            var jobFeatures = model.Transform(data);

            var clienteData = mlContext.Data.LoadFromEnumerable(new[] { new JobInput { Requisitos = cliente.Competencias } });
            var clienteFeatures = model.Transform(clienteData);

            var jobVectors = mlContext.Data.CreateEnumerable<JobVector>(jobFeatures, reuseRowObject: false).ToList();
            var clienteVector = mlContext.Data.CreateEnumerable<JobVector>(clienteFeatures, reuseRowObject: false).First();

            var recomendacoes = jobs.Select((job, idx) => new JobRecomendacao
            {
                Job = job,
                Similaridade = CosineSimilarity(clienteVector.Features, jobVectors[idx].Features)
            })
            .OrderByDescending(r => r.Similaridade)
            .Take(topN)
            .ToList();

            return recomendacoes;
        }

        private float CosineSimilarity(float[] v1, float[] v2)
        {
            float dot = 0, normA = 0, normB = 0;
            for (int i = 0; i < v1.Length; i++)
            {
                dot += v1[i] * v2[i];
                normA += v1[i] * v1[i];
                normB += v2[i] * v2[i];
            }
            return normA == 0 || normB == 0 ? 0 : dot / (float)(System.Math.Sqrt(normA) * System.Math.Sqrt(normB));
        }
    }
}
