namespace DSWI_cibertec_demo.Models
{
    public class MensajeModel
    {
        public int Id { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }
}
