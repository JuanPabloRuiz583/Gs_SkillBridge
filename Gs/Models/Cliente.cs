using System.ComponentModel.DataAnnotations;
namespace Gs.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome não pode estar em branco")]
        [MaxLength(100, ErrorMessage = "Nome não pode exceder 100 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha não pode estar em branco")]
        [MinLength(6, ErrorMessage = "Senha deve ter pelo menos 6 caracteres")]
        public string Senha { get; set; }


        [MaxLength(100, ErrorMessage = "Profissão não pode exceder 100 caracteres")]
        public string ProfissaoAtual { get; set; }

        [Required(ErrorMessage = "As competências são obrigatórias")]
        [MaxLength(300, ErrorMessage = "Competências não podem exceder 300 caracteres")]
        public string Competencias { get; set; }
    }
}
