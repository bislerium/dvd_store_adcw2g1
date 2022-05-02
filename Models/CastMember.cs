using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dvd_store_adcw2g1.Models
{
    public class CastMember
    {
        [Key]
        public int CastMemberNumber { get; set; }


        [ForeignKey("DVDNumber")]
        public DVDTitle DVDTitle { get; set; }



        [ForeignKey("ActorNumber")]
        public Actor Actor { get; set; }

    }
}
