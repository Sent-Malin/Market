using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Data.Models
{
    public class GamesEntity
    {
        public GamesEntity(string idCreator, int countTurns)
        {
            Id = Guid.NewGuid().ToString();
            IdCreator = idCreator;
            CountTurns = countTurns;
            DateEvent = DateTime.Now.Date;
        }

        [Key]
        public string Id { get; set; }
        [ForeignKey("RoomsEntity")]
        public string IdCreator { get; set; }
        public string OperationsPlayer1 { get; set; } = "";
        public string OperationsPlayer2 { get; set; } = "";
        public string OperationsPlayer3 { get; set; } = "";
        public string OperationsPlayer4 { get; set; } = "";
        public int CountTurns { get; set; }
        public int ChangeRatingPlayer1 { get; set; }
        public int ChangeRatingPlayer2 { get; set; }
        public int ChangeRatingPlayer3 { get; set; }
        public int ChangeRatingPlayer4 { get; set; }
        public DateTime DateEvent { get; set; }
    }
}
