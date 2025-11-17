using System.ComponentModel.DataAnnotations;

namespace nexumApp.Models
{
    public class Doacao
    {
        public int Id { get; set; }

        public int MetaId { get; set; }
        public Meta Meta { get; set; }

        // Dados do doador externo (quando não estiver logado)
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public string CPF { get; set; }
        public string Telefone { get; set; }
        public string Descricao { get; set; }

        // Doação por ONG logada
        public int? OngDoadoraId { get; set; }           
        public string OngDoadoraRazaoSocial { get; set; } // Nome/razão social da ONG

        public int Quantidade { get; set; }

        public string Status { get; set; } // Pendente, Confirmada, EmContato, Negada

        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }



}
