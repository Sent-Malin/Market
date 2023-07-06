using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Market.Data.Models
{
    public class OperationsEntity
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public float FrequencyUse { get; set; }
        public float FrequencyWinUse { get; set; }
        public int Cost { get; set; }
        public string Code { get; set; }

        public OperationsEntity(string name, int cost, string code)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Cost = cost;
            Code = code;
        }
    }
}
