using Market_Web.Models;
using Microsoft.AspNetCore.SignalR;

namespace Market_Web.Hubs
{
    public partial class GameHub : Hub
    {
        public List<Room> rooms;

        public GameHub()
        {
            rooms = new List<Room>();
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("On connected rooms:");
            foreach(Room r in rooms)
            {
                Console.WriteLine(r.Name);
            }
            await this.Clients.Caller.SendAsync("ListRooms", this.rooms);
        }
    }
}
