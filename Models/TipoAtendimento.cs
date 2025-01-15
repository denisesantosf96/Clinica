namespace Clinica.Models
{
    public class TipoAtendimento
    {
        public int Id { get; set; }
        public int IdClinica { get; set; } 
        public int IdEspecialidade { get; set; }
        public int IdMedico { get; set; }
        public string Descricao { get; set; }  
        public decimal Valor { get; set; }
        public string? NomeEspecialidade { get; set; }
        public string? NomeMedico { get; set; }
        public string? Especializacao { get; set; }
        public string? Nome { get; set; }
    }
}
