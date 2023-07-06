namespace Market_Web.Models
{
    public class Player
    {
        public string ConnectionId { get; private set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public Market_Rules.Company Company { get; set; }
        public Player(string connectionid, string userid, string name, Market_Rules.Company company=null)
        {
            ConnectionId = connectionid;
            UserId = userid;
            Name = name;
            Company = company;
        }
    }
}
