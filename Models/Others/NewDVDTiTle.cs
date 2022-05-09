using System.ComponentModel.DataAnnotations;

namespace dvd_store_adcw2g1.Models.Others
{
    public class NewDVDTiTle
    {
        [Required]
        public string DVDTitleName { get; set; }

        [Required]
        public String Producer { get; set; }

        [Required]
        public int DVDCategory { get; set; }

        [Required]
        public String Studio { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateReleased { get; set; }

        [Required]
        public List<String> Actors { get; set; }

        [Required]
        public int StandardCharge { get; set; }

        [Required]
        public int PenaltyCharge { get; set; }

        public override string? ToString()
        {
            return $"DVDTitleName = {DVDTitleName}, Producer = {Producer}, DVDCategory = {DVDCategory}, Studio = {Studio}, DateReleased = {DateReleased}, Actors = {String.Join( '|', Actors )}, StandardCharges  = {StandardCharge}, PenaltyCharge = {PenaltyCharge}";
        }
    }
}
