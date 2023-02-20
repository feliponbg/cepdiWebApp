using Microsoft.AspNetCore.Mvc.Rendering;

namespace cepdiWebApp.Models.ViewModels
{
    public class Medicamento
    {
        public long? IIDMEDICAMENTO { get; set; }

        public string NOMBRE { get; set; }

        public string CONCENTRACION { get; set; }

        public int? IIDFORMAFARMACEUTICA { get; set; }

        public float PRECIO { get; set; }

        public int STOCK { get; set; }

        public string PRESENTACION { get; set; }

        public bool BHABILITADO { get; set; }

    }
}
