using System.ComponentModel.DataAnnotations;

namespace cepdiWebApp.Models.ViewModels
{
    public class Sesion
    {
        [Display(Name = "Usuario")]
        public string Usuario { get; set; }

        [Display(Name = "Contraseña")]
        public string Contraseña { get; set; }

        [Display(Name = "Mantener sesión iniciada")]
        public bool MantenerSesionIniciada { get; set; }

    }
}
