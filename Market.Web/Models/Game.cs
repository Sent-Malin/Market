namespace Market_Web.Models
{
    public class Game
    {
        public int Id { get; private set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Player Player3 { get; set; }
        public Player Player4 { get; set; }
        public Market_Rules.Market Market { get; set; }

        public Game(int id, Player player1, Player player2, Player player3, Player player4, Market_Rules.Market market)
        {
            Id = id;
            Player1 = player1;
            Player2 = player2;
            Player3 = player3;
            Player4 = player4;
            Market = market;
        }
    }
}
