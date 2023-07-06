namespace Market_Rules
{
    public class Supplier
    {
        public string Name { get; private set; }
        public int Reputation { get; set; }
        public int BaseQuality { get; set; }
        public int Quality { get; set; }
        public Specialization specialization { get; private set; }
        public Supplier(string name, int repuation, int basequality, Specialization specialization)
        {
            Name = name;
            Reputation = repuation;
            BaseQuality = basequality;
            Quality = basequality;
            this.specialization = specialization;
        }
    }
}
