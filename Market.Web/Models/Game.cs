namespace Market_Web.Models
{
    public class Game
    {
        const int CountPlayer = 4;
        public string Id { get; private set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Player Player3 { get; set; }
        public Player Player4 { get; set; }
        public Market_Rules.Market Market { get; set; }

        //Отличие room от Game в том, что game имеет экземпляр игры, то есть игровой процесс
        //также game связывает компании созданные в factory с реальными игроками
        public Game(string id, Player player1, Player player2, Player player3, Player player4, Market_Rules.Market market)
        {
            Id = id;
            Player1 = player1;
            Player1.Company = market.Companies.ElementAt(0);
            Player2 = player2;
            Player2.Company = market.Companies.ElementAt(1);
            Player3 = player3;
            Player3.Company = market.Companies.ElementAt(2);
            Player4 = player4;
            Player4.Company = market.Companies.ElementAt(3);
            Market = market;
        }

        public int[] GetArrPositions(int[] arrMoney)
        {
            int[] arrPositions = new int[CountPlayer];
            int indexMax = 0;
            for (int j = 1; j < CountPlayer + 1; j++)
            {
                for (int i = 0; i < CountPlayer; i++)
                {
                    if (arrMoney[i] > arrMoney[indexMax])
                        indexMax = i;
                }
                arrPositions[indexMax] = j;
                arrMoney[indexMax] = 0;
            }
            return arrPositions;
        }

        public Dictionary<int, string> GetPositionName(int[] arrMoney)
        {
            Dictionary<int, string> positionName = new Dictionary<int, string>();
            int[] arrPositions = GetArrPositions(arrMoney);
            positionName.Add(arrPositions[0], Player1.Name);
            positionName.Add(arrPositions[1], Player2.Name);
            positionName.Add(arrPositions[2], Player3.Name);
            positionName.Add(arrPositions[3], Player4.Name);
            return positionName;
        }
    }
}
