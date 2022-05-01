using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dvd_store_adcw2g1.Models
{
    public class DVDCopy
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int CopyNumber { get; set; }

        [ForeignKey("DVDNumber")]
        public DVDTitle DVDTitle { get; set; }

        public DateTime DatePurchased { get; set; }

    }
}
