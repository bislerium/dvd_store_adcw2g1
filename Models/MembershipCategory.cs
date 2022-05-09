using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dvd_store_adcw2g1.Models
{
    public class MembershipCategory
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int MembershipCategoryNumber { get; set; }

        public string MembershipCategoryDescription { get; set; }

        public int MembershipCategoryTotalLoans { get; set; }
    }
}
