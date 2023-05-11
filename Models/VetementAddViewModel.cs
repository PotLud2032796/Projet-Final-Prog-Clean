namespace Atelier.Models
{
    public class VetementAddViewModel
    {
        public int VetementId { get; set; }
        public string Nom { get; set; }
        public string? ProprietaireId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string IsTenu2 { get; set; }
        public DateTime DateObtention { get; set; }
        public IFormFile ImageVetement { get; set; }
    }
}
