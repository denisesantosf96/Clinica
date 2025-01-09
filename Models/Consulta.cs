namespace Clinica.Models
{
    public class Consulta
    {
        public int Id { get; set; }
        public int IdClinica { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
    }
}