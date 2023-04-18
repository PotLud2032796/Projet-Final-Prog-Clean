namespace Atelier.Models
{
    public class Vetement
    {
        public int VetementId { get; set; }
        public string Nom { get; set; }
        public string? ProprietaireId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime DateObtention { get; set; }
        public string Image { get; set; }
    }
}
