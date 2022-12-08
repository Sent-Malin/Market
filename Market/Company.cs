namespace Market_Rules
{
    public class Company
    {
        public string Name { get; private set; }
        // Это имеющиеся ресурсы компании
        public int Personal { get; set; }
        public int Bank { get; set; }
        public int Profit { get; set; }
        public int Sales { get; set; }
        public int Reputation { get; set; }
        // Это изменение ресурсов компании, их прирост
        /*
        public int ChangePersonal { get; set; } = 1;
        public int ChangeProfit { get; set; } = 0;
        public int ChangeBank { get; set; } = 0;
        */
        public Supplier Material { get; set; }
        public Supplier Fueller { get; set; }
        public Supplier Transport { get; set; }
        public List<Operation> DoneOperations { get; set; } = new List<Operation> { };

        public Company(string name, int personal, int bank, int profit, 
            int sales, int reputation, Supplier material, Supplier fueller, Supplier transport)
        {
            Name = name;
            Personal = personal;
            Bank = bank;
            Profit = profit;
            Sales = sales;
            Reputation = reputation;
            Material = material;
            Fueller = fueller;
            Transport = transport;
        }
    }
}
