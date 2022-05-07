namespace dvd_store_adcw2g1.Models.ViewModels
{
    public class InActiveLoanMember
    {
        public string MemberFirstName { get; set; }

        public string MemberLastName { get; set; }

        public string MemberAddress { get; set; }

        public DateTime LastDateOut { get; set; }

        public String LastLoanedDVDTitleName { get; set; }

        public int DaysSinceLastLoaned { get; set; }
    }
}
