namespace dvd_store_adcw2g1.Models.ViewModels
{
    public class DVDActors
    { 
        public DVDTitle DVDTitle { get; set; }

        public String Producer { get; set; }

        public String Studio { get; set; }

        public List<Actor> Actors { get; set; }
    }
}
