


using Microsoft.EntityFrameworkCore;
using dvd_store_adcw2g1.Models;

namespace dvd_store_adcw2g1.Models
{
    public class DatabaseContext: DbContext
    {

        public DbSet<User> Users { get; set; } = null!;

        public DbSet<Actor> Actors { get; set; }

        public DbSet<CastMember> CastMembers { get; set; } 

        public DbSet<DVDCategory> DVDCategories { get; set; }

        public DbSet<DVDCopy> DVDCopies { get; set; }

        public DbSet<DVDTitle> DVDTitles { get; set; }

        public DbSet<Loan> Loans { get; set; }

        public DbSet<LoanType> LoanTypes { get; set; }

        public DbSet<Member> Members { get; set; }

        public DbSet<MembershipCategory> MembershipCategories { get; set; }

        public DbSet<Producer> Producers { get; set; }

        public DbSet<Studio> Studios { get; set; }


        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }


        public DbSet<dvd_store_adcw2g1.Models.DVDonShelves> DVDonShelves { get; set; }
    }
}
