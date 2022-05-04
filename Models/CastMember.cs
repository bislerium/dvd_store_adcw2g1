using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dvd_store_adcw2g1.Models
{
    public class CastMember
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int CastMemberNumber { get; set; }

        public int DVDNumber { get; set; }

        public int ActorNumber { get; set; }


        [ForeignKey("DVDNumber")]
        public DVDTitle DVDTitle { get; set; }



        [ForeignKey("ActorNumber")]
        public Actor Actor { get; set; }

    }
}
