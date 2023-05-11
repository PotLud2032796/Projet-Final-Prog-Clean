using Microsoft.AspNetCore.Mvc.Rendering;

namespace Atelier.Models
{
    public class VetementTypeViewModel
    {
        public List<Vetement>? Vetements { get; set; }
        public SelectList? Type { get; set; }
        public string? VetementType { get; set; }
        public string? SearchString { get; set; }
    }
}
