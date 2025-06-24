using Microsoft.EntityFrameworkCore;

namespace CLDV6211_POE_Part_1_ST10438307_Daniel_Gorin.Models
{
    public class EaseDbContext :DbContext
    {
        public EaseDbContext(DbContextOptions<EaseDbContext> context) : base(context) 
        { 
        }   
        public DbSet<Venue> Venue { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<EventType> EventType { get; set; }
    }
}
