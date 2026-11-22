using System.ComponentModel.DataAnnotations;

namespace Info.Models
{
    public class Category
    {
        [Key] // zbędne, bo jest ciąg Id w nazwie klasy, ale zostawiam dla jasności
        [Display(Name = "Identyfikator kategorii:")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Nazwa kategorii jest wymagana.")]
        [Display(Name = "Nazwa kategorii:")]
        [StringLength(50, ErrorMessage = "Nazwa kategorii nie może przekraczać 50 znaków.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Opis kategorii:")]
        [Required(ErrorMessage = "Opis kategorii jest wymagany.")]
        [StringLength(255, ErrorMessage = "Opis kategorii nie może przekraczać 255 znaków.")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Ikona kategorii:")]
        [Required(ErrorMessage = "Ikona kategorii jest wymagana.")]
        [StringLength(30, ErrorMessage = "Ikona kategorii nie może przekraczać 30 znaków.")]
        public string Icon { get; set; } = string.Empty;

        [Display(Name = "Czy aktywna?")]
        public bool Active { get; set; }

        [Display(Name = "Czy wyświetlana?")]
        public bool Display { get; set; }

        // Teksty należące do kategorii
        public ICollection<Text> Texts { get; set; } = new List<Text>();
    }
}
