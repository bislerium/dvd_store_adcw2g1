using System.ComponentModel.DataAnnotations;

namespace dvd_store_adcw2g1.Models
{
    public class Studio
    {
        [Key]
        public int StudioNumber { get; set; }

        public string StudioName { get; set; }
    }
}
