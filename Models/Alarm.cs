namespace PetApp.Models
{
    public class Alarm
    {
        public int IdAlarm { get; set; }
        public int IdUser { get; set; }
        public string Title { get; set; }
        public string Hour { get; set; }
        public string Frequency { get; set; }
    }
}
