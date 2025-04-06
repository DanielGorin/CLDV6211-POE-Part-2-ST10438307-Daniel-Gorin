namespace CLDV6211_POE_Part_1_ST10438307_Daniel_Gorin.Models
{
    public class Venue  //This class will be used to hold data for the Venues table
    {
        public int Id { get; set; } //The automated primary key ID number
        public string Name { get; set; } //The name of the venue
        public string Adress { get; set; } //The adress for the venue
        public int Capacity { get; set; } //The number of guests the venue can hold
        public string ImageURL { get; set; } //A URL that directs to an image for the event
        public bool WheelchairAccess { get; set; } //Wheather the venue is accesable to wheelcahir users
        public bool PetFriendly { get; set; } //Wheather the venue allows pets

    }
}
