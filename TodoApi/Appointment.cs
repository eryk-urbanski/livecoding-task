public class Appointment
{
    public int Id { get; set; }
    public DateTime DateTime { get; set; } // only day and hour
    public Cat? Cat { get; set; }
    public CatOwner? CatOwner { get; set; }
}