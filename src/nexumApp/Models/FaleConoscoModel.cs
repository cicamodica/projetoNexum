using System.ComponentModel.DataAnnotations;

namespace nexumApp.Models;

public enum ContactSubject
{
    [Display(Name = "Sugestão")]
    Sugestao = 1,
    [Display(Name = "Dúvida")]
    Duvida = 2,
    [Display(Name = "Feedback da aplicação")]
    Feedback = 3,
    Outro = 4
}
public class FaleConoscoModel
    {
    public int Id { get; set; }

    [Required, StringLength(120)]
    [Display(Name = "Nome completo")]
    public string Nome { get; set; } = null!;

    [Required, EmailAddress, StringLength(254)]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = null!;

    [Required]
    [Display(Name = "Assunto")]
    public ContactSubject Assunto { get; set; }

    [Required, StringLength(2000, MinimumLength = 5)]
    [Display(Name = "Mensagem")]
    public string Mensagem { get; set; } = null!;

    [Display(Name = "Enviado em")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // flags para filtrar mensagens fale conosco
    public bool Visualizada { get; set; } = false;
    public bool Respondida { get; set; } = false;
    public bool Arquivada { get; set; } = false;

    [StringLength(20)]
    public string Status { get; set; } = "Novo";
    public DateTime? DataResposta { get; set; }
    public string Resposta { get; set; }
}

