using System.ComponentModel.DataAnnotations;

namespace Gs.Dtos
{
    public class JobDTO
    {
        public int Id { get; set; }

    [Required(ErrorMessage = "Título da vaga é obrigatório")]
    [MaxLength(120, ErrorMessage = "Título não pode exceder 120 caracteres")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "Requisitos são obrigatórios")]
    [MaxLength(300, ErrorMessage = "Requisitos não podem exceder 300 caracteres")]
    public string Requisitos { get; set; }

    [Required(ErrorMessage = "Nome da empresa é obrigatório")]
    [MaxLength(100, ErrorMessage = "Nome da empresa não pode exceder 100 caracteres")]
    public string Empresa { get; set; }
    }
}
