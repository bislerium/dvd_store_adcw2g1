using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dvd_store_adcw2g1.Models
{
    public class DVDTitle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DVDNumber { get; set; }

        [Required]
        public string DVDTitleName { get; set; }

        public int ProducerNumber { get; set; }

        public int CategoryNumber { get; set; }

        public int StudioNumber { get; set; }

        [Required]
        [ForeignKey("ProducerNumber")]
        public Producer Producer { get; set; }

        [Required]
        [ForeignKey("CategoryNumber")]
        public DVDCategory DVDCategory { get; set; }

        [Required]
        [ForeignKey("StudioNumber")]
        public Studio Studio { get; set; }

        [Required]
        public DateTime DateReleased { get; set; }

        [Required]
        public int StandardCharge { get; set; }

        [Required]
        public int PenaltyCharge { get; set; }
    }
}
