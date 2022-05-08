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

        [Required]
        [ForeignKey("MembershipCategoryNumber")]
        public MembershipCategory MembershipCategory { get; set; }

        [Required]
        public string MembershipLastName { get; set; }

        [Required]
        public string MembershipFirstName { get; set; }

        [Required]
        public string MembershipAddress { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime MemberDOB { get; set; }
    }
}
