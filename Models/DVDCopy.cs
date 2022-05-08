using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dvd_store_adcw2g1.Models
{
    public class DVDCopy
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int CopyNumber { get; set; }

        public int DVDNumber { get; set; }

        [ForeignKey("DVDNumber")]
        [Required]
        public DVDTitle DVDTitle { get; set; }

        [Required]
        public DateTime DatePurchased { get; set; }

    }
}
