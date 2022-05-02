using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dvd_store_adcw2g1.Models
{
    public class DVDTitle
    {
        [Key]
        public int DVDNumber { get; set; }

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
