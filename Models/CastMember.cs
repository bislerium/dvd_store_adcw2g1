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

        [Required]
        [ForeignKey("DVDNumber")]
        public DVDTitle DVDTitle { get; set; }

        [Required]
        [ForeignKey("ActorNumber")]
        public Actor Actor { get; set; }

    }
}
