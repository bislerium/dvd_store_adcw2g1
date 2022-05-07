namespace dvd_store_adcw2g1.Models.ViewModels
{
    public class MembersActiveLoans
    {
        public Member Member { get; set; }

        public MembershipCategory MemberCategory { get; set; }

        public int TotalActiveLoans { get; set; }
    }
}
