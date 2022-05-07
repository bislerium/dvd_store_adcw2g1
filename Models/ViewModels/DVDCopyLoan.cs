namespace dvd_store_adcw2g1.Models.ViewModels
{
    public class DVDCopyLoan
    {
        public string DVDTitleName { get; set; }
        public int CopyNumber { get; set; }
        public string MemberName { get; set; }
        public DateTime DateOut { get; set; }
        public int TotalLoans { get; set; }
    }
}
