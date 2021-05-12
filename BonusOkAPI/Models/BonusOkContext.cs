using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BonusOkAPI.Models
{
    public class BonusOkContext : DbContext
    {
        public BonusOkContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {}
        
        public DbSet<Client> Clients { get; set; }
        
        public DbSet<Card> Cards { get; set; }
        
        public DbSet<Promo> Promos { get; set; }
    }
}