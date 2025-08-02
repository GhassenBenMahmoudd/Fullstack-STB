using System.ComponentModel.DataAnnotations;

public class LoginDto
{
    [Required]
    [StringLength(50)] // Correspond à la longueur de la Matricule dans votre entité Employe
    public string Matricule { get; set; } // <-- MODIFIÉ: Email a été remplacé par Matricule

    [Required]
    public string Password { get; set; }
}
