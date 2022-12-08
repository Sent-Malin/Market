namespace Market_Rules
{
    // Отображает состояние игры на различное кол-во ходов в будущем
    // то есть влияние событий - новостей и исследований в перспективе
    public delegate void EndEffect(Company[] companies);
    public class Condition
    {
        public int CountTurn { get; set; } = 1;
        public int TemporaryTimeChange { get; set; } = 0;
        public int TemporaryPersonalChange { get; set; } = 0;
        public int TemporaryBankChange { get; set; } = 0;
        public int TemporaryProfitChange { get; set; } = 0;
        public int TemporarySalesChange { get; set; } = 0;
        public int TemporaryReputationChange { get; set; } = 0;
        
        public EndEffect? endEffect=null;
        public List<Company> CompanyUnderEffect { get; set; }=new List<Company>();
    }
}
