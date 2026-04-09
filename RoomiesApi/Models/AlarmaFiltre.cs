namespace RoomiesApi.Models
{
    public class AlarmaFiltre
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Gen { get; set; }
        public string? Zona { get; set; }
        public string? Buget { get; set; }
        public string? StilViata { get; set; }
        public string? Preferinte { get; set; }
    }
}