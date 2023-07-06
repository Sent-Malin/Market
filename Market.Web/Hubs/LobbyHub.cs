using Market.Data.Models;
using Market_Init;
using Market_Rules;
using Market_Web.Controllers;
using Market_Web.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

namespace Market_Web.Hubs
{
    public partial class GameHub
    {
        public async Task<Player> CreateRoom(string nameRoom, string userId, string namePlayer, string password="")
        {
            Player player = new Player(this.Context.ConnectionId, userId, namePlayer);
            Room room;
            if (password != "")
            {
                room = new Room(nameRoom, player, true);
            }
            else
            {
                room = new Room(nameRoom, player, false);
            }
            this.rooms.Add(room);
            RoomsEntity entity = new RoomsEntity(player.UserId, nameRoom, password);
            db.Rooms.Add(entity);
            db.SaveChanges();
            await this.Clients.All.SendAsync("ListRooms", this.rooms);
            return player;
        }

        //Добавляет игрока в целевую комнату, удаляя его из существующей
        public async Task<Player> JoinRoom(string namePlayer, string userId, string idHost, string password)
        {
            Player player = new Player("", "", "");
            var searchRoom=this.rooms
                .Where(r=>(r.Player2.UserId == userId) ||(r.Player3.UserId == userId) ||(r.Player4.UserId == userId))
                .FirstOrDefault();
            RoomsEntity? roomEntity = db.Rooms
                .Where(r => (r.IdPlayer2 == userId) || (r.IdPlayer3 == userId) || (r.IdPlayer4 == userId))
                .FirstOrDefault();
            var targetRoomEntity = db.Rooms.Where(r => r.IdCreator == idHost).First();
            if (targetRoomEntity.Password == password)
            {
                if ((searchRoom == null) || (roomEntity == null))
                {
                    player = new Player(this.Context.ConnectionId, userId, namePlayer);
                }
                else
                {
                    roomEntity.CountPlayer -= 1;
                    if (searchRoom.Player2.UserId == userId)
                    {
                        player = searchRoom.Player2;
                        roomEntity.IdPlayer2 = "";
                        searchRoom.Player2 = new Player("", "", "");
                    }
                    if (searchRoom.Player3.UserId == userId)
                    {
                        player = searchRoom.Player3;
                        roomEntity.IdPlayer3 = "";
                        searchRoom.Player3 = new Player("", "", "");
                    }
                    if (searchRoom.Player4.UserId == userId)
                    {
                        player = searchRoom.Player4;
                        roomEntity.IdPlayer4 = "";
                        searchRoom.Player4 = new Player("", "", "");
                    }
                }
                var room = this.rooms.Where(i => i.Player1.UserId == idHost).First();
                
                if (room.Player2.ConnectionId == "")
                {
                    targetRoomEntity.IdPlayer2 = player.UserId;
                    room.Player2 = player;
                }
                else if (room.Player3.ConnectionId == "")
                {
                    targetRoomEntity.IdPlayer3 = player.UserId;
                    room.Player3 = player;
                }
                else if (room.Player4.ConnectionId == "")
                {
                    targetRoomEntity.IdPlayer4 = player.UserId;
                    room.Player4 = player;
                }
                targetRoomEntity.CountPlayer += 1;
                db.SaveChanges();
                await Clients.All.SendAsync("ListRooms", this.rooms);
                return player;
            }
            else
            {
                await Clients.Caller.SendAsync("WrongPassword", idHost);
                return player;
            }
        }

        public async Task StartGame(string userId, string connectionIdHost)
        {
            var searchRoom = this.rooms.Where(r => r.Player1.UserId == userId).First();
            Market_Rules.Market market=Factory.GetMarket(userId);
            Game game = new Game(userId, searchRoom.Player1, searchRoom.Player2,
                searchRoom.Player3, searchRoom.Player4, market);
            this.games.Add(game);
            await Task.WhenAll(
                this.Groups.AddToGroupAsync(game.Player1.ConnectionId, groupName: userId),
                this.Groups.AddToGroupAsync(game.Player2.ConnectionId, groupName: userId),
                this.Groups.AddToGroupAsync(game.Player3.ConnectionId, groupName: userId),
                this.Groups.AddToGroupAsync(game.Player4.ConnectionId, groupName: userId),
                this.Clients.Group(userId).SendAsync("Start", game));
            db.Rooms.Where(r => r.IdCreator == userId).First().Status="InProgress";
            db.Games.Add(new GamesEntity(userId, CountTurn));
            db.SaveChanges();
            rooms.Remove(searchRoom);
            await Clients.AllExcept(game.Player1.ConnectionId, game.Player2.ConnectionId,
                game.Player3.ConnectionId, game.Player4.ConnectionId).SendAsync("ListRooms", this.rooms);
            // Подписываем BaseHub на событие - начало хода в игре
            game.Market.StartTurnEvent += StartTurn;
            // Подписываем BaseHub на событие - конец игры
            game.Market.EndGameEvent += EndGame;
            game.Market.Game();
        }

        public async Task Exit(string userId)
        {
            db.Users.Where(u => u.Id == userId).First().IsOnline = false;
            db.SaveChanges();
            Room? r = rooms.Where(r => r.Player1.UserId == userId).FirstOrDefault();
            if (r != null) { rooms.Remove(r); } else
            {
                var searchRoom = this.rooms
                    .Where(r => (r.Player2.UserId == userId) || (r.Player3.UserId == userId) || (r.Player4.UserId == userId))
                    .FirstOrDefault();
                RoomsEntity? roomEntity = db.Rooms
                    .Where(r => (r.IdPlayer2 == userId) || (r.IdPlayer3 == userId) || (r.IdPlayer4 == userId))
                    .FirstOrDefault();
                if ((searchRoom != null) && (roomEntity != null))
                {
                    roomEntity.CountPlayer -= 1;
                    if (searchRoom.Player2.UserId == userId)
                    {
                        roomEntity.IdPlayer2 = "";
                        searchRoom.Player2 = new Player("", "", "");
                    }
                    if (searchRoom.Player3.UserId == userId)
                    {
                        roomEntity.IdPlayer3 = "";
                        searchRoom.Player3 = new Player("", "", "");
                    }
                    if (searchRoom.Player4.UserId == userId)
                    {
                        roomEntity.IdPlayer4 = "";
                        searchRoom.Player4 = new Player("", "", "");
                    }
                    db.SaveChanges();
                    await Clients.All.SendAsync("ListRooms", this.rooms);
                }
            }
        }
    }
}
