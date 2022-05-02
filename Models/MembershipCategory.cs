using System.ComponentModel.DataAnnotations;

namespace dvd_store_adcw2g1.Models
{
    public class MembershipCategory
    {
        [Key]
        public int MembershipCategoryNumber { get; set; }

        public string MembershipCategoryDescription { get; set; }

        public int MembershipCategoryTotalLoans { get; set; }
    }
}
