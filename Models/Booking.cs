namespace CLDV6211_POE_Part_1_ST10438307_Daniel_Gorin.Models
{
    public class Booking
    {
        public int Id { get; set; } //The automated primary key ID number
        public int VenueId { get; set; } // The ID for the associated venue
        public int EventId { get; set; }// The ID for teh associated event
        public DateTime BookingDate { get; set; }// the date the booking was made
    }
}
