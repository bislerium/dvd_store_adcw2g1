using System.ComponentModel.DataAnnotations;

namespace dvd_store_adcw2g1.Models
{
    public class DVDCategory
    {
        [Key]
        public int CategoryNumber { get; set; }

        public string CategoryDescription { get; set; }

        public string AgeRestricted { get; set; }
    }
}
