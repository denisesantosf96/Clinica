namespace Clinica.Models
{
    public class Pessoa
    {
        public int Acao { get; set; }
        public string Opcao { get; set; }
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string RG { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string CEP { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Especializacao { get; set; }
        public string CRM { get; set; }
        public int IdConsulta { get; set; }
        public int IdClinica { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
    }
}