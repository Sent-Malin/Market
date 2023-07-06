using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Data.Models
{
    public class UsersEntity
    {
        [Key]
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public bool IsOnline { get; set; }
        public string? ConnectionId { get; set; }
        public int CountGames { get; set; }
        public int CountWin { get; set; }
        public int Rating { get; set; }
        public DateTime DateRegistration { get; set; }

        public UsersEntity(string name, string password, int countGames, int countWin, int rating)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Password = password;
            IsOnline = true;
            ConnectionId = "";
            CountGames = countGames;
            CountWin = countWin;
            Rating = rating;
            DateRegistration = DateTime.Now;
        }

        public UsersEntity() { }
    }
}
