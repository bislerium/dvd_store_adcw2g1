using System.ComponentModel.DataAnnotations;

namespace dvd_store_adcw2g1.Models
{
    public class User
    {
        [Key]
        public int UserNumber { get; set; }

        public string UserName { get; set; }

        public string UserType { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(100, MinimumLength = 5,
           ErrorMessage = "Title must be between 5 and 100 characters long")]
        public string UserPassword { get; set; }

    }
}
