using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dvd_store_adcw2g1.Models
{
    public class Loan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LoanNumber { get; set; }

        public int LoanTypeNumber { get; set; }

        public int CopyNumber { get; set; }

        public int MemberNumber { get; set; }

        [ForeignKey("LoanTypeNumber")]
        [Required]
        public LoanType LoanType { get; set; }

        [ForeignKey("CopyNumber")]
        [Required]
        public DVDCopy DVDCopy { get; set; }

        [ForeignKey("MemberNumber")]
        [Required]
        public Member Member { get; set; }

        [Required]
        public DateTime DateOut { get; set; }

        [Required]
        public DateTime DateDue { get; set; }

        public DateTime? DateReturned { get; set; }
    }
}
