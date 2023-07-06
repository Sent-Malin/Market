using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Data.Models
{
    public class RoomsEntity
    {
        public RoomsEntity(string idCreator, string name, string? password)
        {
            Id = Guid.NewGuid().ToString();
            IdCreator = idCreator;
            Name = name;
            Status = "wait";
            Password = password;
            CountPlayer = 1;
        }

        [Key]
        public string? Id { get; set; }
        [ForeignKey("UsersEntity")]
        public string IdCreator { get; set; }
        public string Name { get; set; }
        [ForeignKey("UsersEntity")]
        public string? IdPlayer2 { get; set; }
        [ForeignKey("UsersEntity")]
        public string? IdPlayer3 { get; set; }
        [ForeignKey("UsersEntity")]
        public string? IdPlayer4 { get; set; }
        public string Status { get; set; }
        public string? Password { get; set; }
        public int CountPlayer { get; set; }
    }
}
