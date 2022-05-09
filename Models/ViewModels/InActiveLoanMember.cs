namespace dvd_store_adcw2g1.Models.ViewModels
{
    public class InActiveLoanMember
    {
        public Member Member { get; set; }

        public DateTime LastDateOut { get; set; }

        public String LastLoanedDVDTitleName { get; set; }

        public double DaysSinceLastLoaned { get; set; }
    }
}
