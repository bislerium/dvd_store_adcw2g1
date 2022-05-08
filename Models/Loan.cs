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

        [Required]
        [ForeignKey("LoanTypeNumber")]
        public LoanType LoanType { get; set; }

        [Required]
        [ForeignKey("CopyNumber")]
        public DVDCopy DVDCopy { get; set; }

        [Required]
        [ForeignKey("MemberNumber")]
        public Member Member { get; set; }

        [Required]
        public DateTime DateOut { get; set; }

        [Required]
        public DateTime DateDue { get; set; }

        public DateTime? DateReturned { get; set; }
    }
}
