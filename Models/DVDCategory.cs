using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dvd_store_adcw2g1.Models
{
    public class DVDCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryNumber { get; set; }

        [Required]
        public string CategoryDescription { get; set; }

        [Required]
        public string AgeRestricted { get; set; }
    }
}
