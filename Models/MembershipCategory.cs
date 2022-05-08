using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dvd_store_adcw2g1.Models
{
    public class MembershipCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MembershipCategoryNumber { get; set; }

        [Required]
        public string MembershipCategoryDescription { get; set; }

        [Required]
        public int MembershipCategoryTotalLoans { get; set; }
    }
}
