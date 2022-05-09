namespace dvd_store_adcw2g1.Models.ViewModels
{
    public class RecentDVDCopyLoan
    {
        public DVDTitle DVDTitle { get; set; }
        public Member Member { get; set; } 
        public Loan Loan { get; set; } 
    }
}
