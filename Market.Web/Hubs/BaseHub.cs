using Market_Web.Models;
using Microsoft.AspNetCore.SignalR;
using Market.Data;
using Market.Data.Models;
using Market_Init;

namespace Market_Web.Hubs
{
    public partial class GameHub : Hub
    {
        const int CountPlayer = 4;
        const int CountTurn = 12;
        const int MaxRatingPlus = 50;
        const int RatingDownStep = 25;
        private List<Room> rooms;
        private List<Game> games;
        public MarketDbContext db = new MarketDbContext();

        public GameHub()
        {
            rooms = new List<Room>();
            games = new List<Game>();
            if (db.Operations.Select(t => t).FirstOrDefault() == null)
            {
                Market_Rules.Market market = Factory.GetMarket("_");
                foreach (Market_Rules.Operation o in market.Operations)
                {
                    OperationsEntity op = new OperationsEntity(o.Name, o.Cost, o.Code);
                    db.Operations.Add(op);
                }
            }
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ListRooms", this.rooms, this.Context.ConnectionId);
        }
    }
}
