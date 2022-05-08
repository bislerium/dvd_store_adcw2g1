using System.ComponentModel.DataAnnotations;

namespace dvd_store_adcw2g1.Models
{
    public class DVDonShelves
    {
        [Key]
        public int DVDonShelvesnumber { get; set; }
        public DVDTitle DVDTitle { get; set; } = null!;

        public Actor ActorSurname { get; set; }

    }
}
