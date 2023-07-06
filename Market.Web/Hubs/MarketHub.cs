using Market.Data.Models;
using Market_Rules;
using Market_Web.Models;
using Microsoft.AspNetCore.SignalR;

namespace Market_Web.Hubs
{
    public partial class GameHub
    {
        public async Task StartTurn(string idSender, New news, int thisTurnTime)
        {
            Game game = this.games.Where(r=>r.Player1.UserId==idSender).First();
            await this.Clients.Group(idSender).SendAsync("StartTurn", game, news, thisTurnTime);
        }
        public async Task EndGame(string idSender)
        {
            Game endedGame = games.Where(g => g.Id == idSender).First();
            //Регулировка статистики
            int[] points = endedGame.Market.CalculationPoints();
            Dictionary<int, string> positionName = endedGame.GetPositionName(points);
            UsersEntity? user;
            for (int i=0; i<CountPlayer; i++)
            {
                user = db.Users.Where(u => u.Name == positionName[i + 1]).FirstOrDefault();
                if(user != null)
                {
                    if (i == 0)
                        user.CountWin += 1;
                    user.CountGames += 1;
                    user.Rating += MaxRatingPlus - RatingDownStep * i;
                    db.SaveChanges();
                }
            }
            int[] arrPositions=endedGame.GetArrPositions(points);
            var dbGame = db.Games.Where(g => g.IdCreator == idSender).First();
            dbGame.ChangeRatingPlayer1 = MaxRatingPlus - RatingDownStep * arrPositions[0];
            dbGame.ChangeRatingPlayer2 = MaxRatingPlus - RatingDownStep * arrPositions[1];
            dbGame.ChangeRatingPlayer3 = MaxRatingPlus - RatingDownStep * arrPositions[2];
            dbGame.ChangeRatingPlayer4 = MaxRatingPlus - RatingDownStep * arrPositions[3];
            
            this.games.Remove(endedGame);
            db.Rooms.Where(r => r.IdCreator == idSender).First().Status = "Closed";
            db.SaveChanges();
            await this.Clients.Group(idSender).SendAsync("EndGame", endedGame, endedGame.Market.CalculationPoints());
        }

        public async Task SendMessage(string gameId, string playerName, string textMessage)
        {
            await this.Clients.Group(gameId).SendAsync("UpdateChat", playerName, textMessage);
        }
        public async Task ChangeSupplier(string gameId, string connectionId, int numberSupplier)
        {
            Game game = this.games.Where(g => g.Id == gameId).First();
            if (game.Player1.ConnectionId == connectionId) {
                game.Market.ChooseSupplier(game.Player1.Company, game.Market.Suppliers[numberSupplier]);
            }
            else if (game.Player2.ConnectionId == connectionId)
            {
                game.Market.ChooseSupplier(game.Player2.Company, game.Market.Suppliers[numberSupplier]);
            }
            else if (game.Player3.ConnectionId == connectionId)
            {
                game.Market.ChooseSupplier(game.Player3.Company, game.Market.Suppliers[numberSupplier]);
            }
            else if (game.Player4.ConnectionId == connectionId)
            {
                game.Market.ChooseSupplier(game.Player4.Company, game.Market.Suppliers[numberSupplier]);
            }
            await this.Clients.Group(gameId).SendAsync("UpdateGame", game);
        }

        public async Task DoOperation(string playerId, string operationName, string gameId)
        {
            Game game = games.Where(g => (g.Player1.UserId == playerId) || (g.Player2.UserId == playerId) ||
            (g.Player3.UserId == playerId) || (g.Player4.UserId == playerId)).First();
            Company? c = null;
            Market_Rules.Operation oper = game.Market.Operations.Where(o => o.Name == operationName).First();
            if (game.Player1.UserId==playerId)
            {
                c = game.Player1.Company;
                db.Games.Where(g => g.IdCreator == game.Id).First().OperationsPlayer1 += oper.Code;
            } else if (game.Player2.UserId == playerId)
            {
                c = game.Player2.Company;
                db.Games.Where(g => g.IdCreator == game.Id).First().OperationsPlayer2 += oper.Code;
            } else if(game.Player3.UserId == playerId)
            {
                c = game.Player3.Company;
                db.Games.Where(g => g.IdCreator == game.Id).First().OperationsPlayer3 += oper.Code;
            } else if (game.Player4.UserId == playerId)
            {
                c = game.Player4.Company;
                db.Games.Where(g => g.IdCreator == game.Id).First().OperationsPlayer4 += oper.Code;
            }
            game.Market.Operation(c, oper);
            db.SaveChanges();
            await this.Clients.Group(gameId).SendAsync("UpdateOperation", game);
        }
    }
}
