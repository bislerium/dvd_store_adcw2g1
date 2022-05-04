using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dvd_store_adcw2g1.Models
{
    public class LoanType
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int LoanTypeNumber { get; set; }

        public string LoanTypeName { get; set; }

        public string LoanDuration { get; set; }
    }
}
