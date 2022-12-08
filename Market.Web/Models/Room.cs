namespace Market_Web.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Player Player3 { get; set; }
        public Player Player4 { get; set; }
        public bool IsInGame { get; set; } = false;
    }
}
