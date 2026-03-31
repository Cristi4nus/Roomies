namespace RoomiesApi.Models
{
    public class Mesaj
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Text { get; set; }
        public DateTime Data { get; set; }
    }
}