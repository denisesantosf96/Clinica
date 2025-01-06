namespace Clinica.Models
{
    public class Agendamento
    {
        public int Id { get; set; }
        public int IdConsulta { get; set; }
        public int IdMedico { get; set; }
        public int IdPaciente { get; set; }
        public DateTime Data { get; set; }
        public string Horario { get; set; }
        public string FormaPagamento { get; set; }
        public int IdClinica { get; set; }
        
    }
}