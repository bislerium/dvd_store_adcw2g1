using System.ComponentModel.DataAnnotations;

namespace dvd_store_adcw2g1.Models
{
    public class CastMember
    {
        [Key]
        public int DVDNumber { get; set; }

        public int ActorNumber { get; set; }

    }
}
