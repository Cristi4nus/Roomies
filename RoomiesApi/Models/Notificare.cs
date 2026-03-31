namespace RoomiesApi.Models
{
    public class Notificare
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; }
        public DateTime Data { get; set; }
    }
}
