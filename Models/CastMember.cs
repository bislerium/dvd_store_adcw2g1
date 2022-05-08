using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dvd_store_adcw2g1.Models
{
    public class CastMember
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CastMemberNumber { get; set; }

        public int DVDNumber { get; set; }

        public int ActorNumber { get; set; }


        [ForeignKey("DVDNumber")]
        [Required]
        public DVDTitle DVDTitle { get; set; }


        [ForeignKey("ActorNumber")]
        [Required]
        public Actor Actor { get; set; }

    }
}
