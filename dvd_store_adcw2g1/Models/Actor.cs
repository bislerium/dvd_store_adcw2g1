using System.ComponentModel.DataAnnotations;

namespace dvd_store_adcw2g1.Models
{
    public class Actor
    {
        [Key]
        public int ActorNumber { get; set; }

        public string ActorSurname { get; set; }

        public string ActorFirstName { get; set; }
    }
}
