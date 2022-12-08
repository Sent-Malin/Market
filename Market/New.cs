namespace Market_Rules
{
    public delegate Condition NewsEffect(Company[] companies);
    public class New
    {
        public string Title { get; private set; }
        public string Description { get; private set; }

        public NewsEffect effect;
        public New(string title, string description, NewsEffect effect)
        {
            Title = title;
            Description = description;
            this.effect = effect;
        }
    }
}
