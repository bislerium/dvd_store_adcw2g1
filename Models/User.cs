using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dvd_store_adcw2g1.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserNumber { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string UserType { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(30, MinimumLength = 5,
           ErrorMessage = "Title must be between 5 and 30 characters long")]
        public string UserPassword { get; set; }
    }
}
