using Microsoft.AspNetCore.SignalR;
namespace Market_Web.Hubs
{
    public partial class GameHub
    {
        public async Task Send(string message)
        {
            await this.Clients.All.SendAsync("Recieve", message);
        }
    }
}
