﻿namespace Market_Rules
{
    public delegate Condition OperationEffect(Company[] companies, Company company);
    public class Operation
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int Cost { get; set; }

        public OperationEffect effect;
        public Operation(string name, string description, int cost, OperationEffect effect)
        {
            Name = name;
            Description = description;
            Cost = cost;
            this.effect = effect;
        }
    }
}
