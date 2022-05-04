using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dvd_store_adcw2g1.Models
{
    public class Member
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int MemberNumber { get; set; }

        public int MembershipCategoryNumber { get; set; }

        [ForeignKey("MembershipCategoryNumber")]
        public MembershipCategory MembershipCategory { get; set; }

        public string MembershipLastName { get; set; }

        public string MembershipFirstName { get; set; }

        public string MembershipAddress { get; set; }

        public DateTime MemberDOB { get; set; }
    }
}
