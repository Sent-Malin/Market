using Market_Init;
using Market_Rules;
using Market_Web.Models;
using Microsoft.AspNetCore.SignalR;

namespace Market_Web.Hubs
{
    public partial class GameHub
    {
        public async Task CreateRoom(string name)
        {
            Player player = new Player(this.Context.ConnectionId, this.Context.UserIdentifier, "Player1");
            Room room = new Room();
            room.Name = name;
            room.Player1 = player;
            this.rooms.Add(room);
            foreach (Room r in rooms)
            {
                Console.WriteLine(r.Name);
            }
            Console.WriteLine("added " + room.Name);
            await this.Clients.All.SendAsync("DrawRoom", name, player);
        }
    }
}
