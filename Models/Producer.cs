using System.ComponentModel.DataAnnotations;

namespace dvd_store_adcw2g1.Models
{
    public class Producer
    {
        [Key]
        public int ProducerNumber { get; set; }

        public string ProducerName { get; set; }

    }
}
