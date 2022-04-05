using System.ComponentModel.DataAnnotations;

namespace dvd_store_adcw2g1.Models
{
    public class LoanType
    {
        [Key]
        public int LoanTypeNumber { get; set; }

        public string LoanTypeName { get; set; }

        public string LoanDuration { get; set; }
    }
}
