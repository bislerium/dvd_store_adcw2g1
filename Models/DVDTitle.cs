using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dvd_store_adcw2g1.Models
{
    public class DVDTitle
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int DVDNumber { get; set; }

        public string DVDTitleName { get; set; }

        public int ProducerNumber { get; set; }

        public int CategoryNumber { get; set; }

        public int StudioNumber { get; set; }

        [ForeignKey("ProducerNumber")]
        public Producer Producer { get; set; }

        [ForeignKey("CategoryNumber")]
        public DVDCategory DVDCategory { get; set; }

        [ForeignKey("StudioNumber")]
        public Studio Studio { get; set; }

        public DateTime DateReleased { get; set; }

        public int StandardCharge { get; set; }

        public int PenaltyCharge { get; set; }
    }
}
