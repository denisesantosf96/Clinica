namespace Clinica.Models
{
    public class Agendamento
    {
        public int Id { get; set; }
        public int IdTipoAtendimento { get; set; }
        public int IdPaciente { get; set; }
        public DateTime DataHora { get; set; } = DateTime.Now;
        public DateTime? DataHoraConfirmacao { get; set; } 
        public bool EstevePresente { get; set; }
        public int IdClinica { get; set; }
        public string? NomePaciente { get; set; }
        public string? NomeEspecialidade { get; set; }
        public string? NomeMedico { get; set; }
    }
} 