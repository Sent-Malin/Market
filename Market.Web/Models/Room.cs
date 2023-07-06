namespace Market_Web.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Password { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Player Player3 { get; set; }
        public Player Player4 { get; set; }

        public Room(string name, Player player1, bool password=false)
        {
            Name = name;
            Password = password;
            Player1 = player1;
            Player2 = new Player("", "", "");
            Player3 = new Player("", "", "");
            Player4 = new Player("", "", "");
        }
    }
}
