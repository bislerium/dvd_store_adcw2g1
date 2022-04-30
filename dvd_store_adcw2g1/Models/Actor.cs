using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dvd_store_adcw2g1.Models
{
    public class Actor
    {
        [Key]

        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ActorNumber { get; set; }

        public string ActorSurname { get; set; }

        public string ActorFirstName { get; set; }
    }
}
