namespace CLDV6211_POE_Part_1_ST10438307_Daniel_Gorin.Models
{
    public class Event
    {
        public int Id { get; set; } //The automated primary key ID number
        public string Name { get; set; } //The name of the event
        public DateTime Date { get; set; } //The date the event is cheduled for
        public string Description { get; set; } //A description of the event
        public string ImageURL { get; set; } //A URL that directs to an image for the event
    }
}
