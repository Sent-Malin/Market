namespace Market_Rules;

public class Market
{
    const int CountPlayer = 4;
    const int CountTurn = 12;
    const int TimeTurn = 20000;
    const int HundredPercent = 100;
    const int DownQualityPercent = 15;

    private int FondMoney = 1000;
    private object locker = new();

    public string Id { get; private set; }
    public int CurrentTurn { get; private set; } = 0;
    public Company[] Companies { get; private set; } = new Company[3];
    // Списки доступных новостей, поставщиков и исследований и состояний
    public List<New> News { get; private set; }
    public List<Supplier> Suppliers { get; private set; }
    public List<Operation> Operations { get; private set; }
    public List<Condition> Conditions { get; private set; } = new List<Condition>();

    // Событие для оповещений о:

    // начале хода, передается id игры, у которой состояние изменилось
    // новость, произошедшая в начале хода и время этого хода
    public delegate Task StartTurnHandler(string idSender, New news, int thisTurnTime);
    public event StartTurnHandler StartTurnEvent;

    // конце хода, передается id игры, у которой состояние изменилось
    // новость, произошедшая в начале хода
    public delegate Task EndTurnHandler(string idSender);
    public event EndTurnHandler EndTurnEvent;

    // конце игры, передается id игры, у которой состояние изменилось
    public delegate Task EndGameHandler(string idSender);
    public event EndGameHandler EndGameEvent;

    public Market(string id, Company[] companies, List<New> news, 
        List<Supplier> suppliers, List<Operation> operations, int fondMoney)
    {
        Id = id;
        Companies = companies;
        News = news;
        Suppliers = suppliers;
        Operations = operations;
        FondMoney = fondMoney;
    }

    /*
        Game()
        Дальше ходы идут сами,
        действия игроков - будут осуществляться сервером, вручную
        обращаясь к экземпляру Market. 
        Задача Market - играть ходы, выдавать оповещения.
        Conditions берутся из new и operation возвратом
    */
    // Главный цикл игры, запускается асинхронно, чтобы мы могли управлять
    // состояниями игры во время этого цикла ходов
    public void Game() {
        Task t = new Task(() =>
        {
            SupplierRestatement();

            while (CurrentTurn != CountTurn)
            {
                CurrentTurn++;
                New nowNew = DoNews();
                // Чистим отработавшие состояния и уменьшаем счетчики текущих и
                // считаем время этого хода исходя из состояний
                int thisTurnTime = TimeTurn;
                lock (locker)
                {
                    List<Condition> cl = new List<Condition>();
                    cl.AddRange(Conditions);
                    foreach (Condition c in cl)
                    {
                        if (c.CountTurn == 0) 
                            Conditions.Remove(c);
                        else 
                            thisTurnTime += c.TemporaryTimeChange;
                        c.CountTurn--;
                    }
                    cl.Clear();
                    InitStartTurnParametrs();
                    //Вызываем событие - начало хода
                    if (StartTurnEvent is not null) 
                        StartTurnEvent(this.Id, nowNew, thisTurnTime);
                }
                Thread.Sleep(thisTurnTime);

                if(EndTurnEvent is not null) 
                    EndTurnEvent(Id);
            }
            EndGameEvent(this.Id);
        });
        t.Start();
    }

    // Метод совершающий следующий ход
    private void InitStartTurnParametrs()
    {
        // Обновляем данные компаний перед ходом
        foreach (Company c in Companies)
        {
            // Учитываем все состояния
            foreach(Condition cond in Conditions)
            {
                if (cond.CompanyUnderEffect.Contains(c))
                {
                    // Изменяем состояние компании, если на неё влияет состояние игры
                    c.Personal += cond.TemporaryPersonalChange;
                    c.Bank += cond.TemporaryBankChange;
                    c.Profit += cond.TemporaryProfitChange;
                    c.Sales += cond.TemporarySalesChange;
                    c.Reputation += cond.TemporaryReputationChange;

                    if (cond.CountTurn == 0 && cond.endEffect!=null)
                    {
                        cond.endEffect.Invoke(Companies);
                    }
                }
            }
            c.Bank += c.Profit;
            c.Sales = (c.Fueller.Quality+ c.Material.Quality+c.Transport.Quality + 
                c.Personal + c.Reputation) / 5 * FondMoney / HundredPercent;
            c.Profit = c.Sales - c.Personal * 3;
            c.Personal += 1;
            c.Reputation += c.Personal + CalcReputationSuppl(c);
        }
    }

    private New DoNews()
    {
        Random rnd = new Random();
        int value = rnd.Next(0, News.Count);
        Condition newCond=News[value].effect.Invoke(Companies);
        if(newCond is not null)
            Conditions.Add(newCond);

        return News[value];
    }

    // Методы применяемые игроком, вызываются сервером

    // Применение исследования компанией
    public string Operation(Company c, Operation s)
    {
        if ((c.Bank >= s.Cost) &&(c!=null))
        {
            Condition operationCond = s.effect.Invoke(Companies, c);
            if (operationCond is not null)
                Conditions.Add(operationCond);
            c.DoneOperations.Add(s.Name);
            c.Bank -= s.Cost;

            return "success";
        }
        else 
        { 
            return "not enough money"; 
        }
    }

    // Смена поставщика компанией
    public void ChooseSupplier(Company c, Supplier s)
    {
        switch (s.specialization)
            { 
            case (Specialization)1:
                c.Material = s;
                break;
            case (Specialization)2:
                c.Fueller = s;
                break;
            case (Specialization)3:
                c.Transport = s;
                break;
            default: throw new ArgumentException("Spec is null");
            }
        SupplierRestatement();
    }

    // Вспомогательные функции
    // Считает влияние репутации поставщиков
    private int CalcReputationSuppl(Company c)
    {
        float res = c.Fueller.Reputation + c.Transport.Reputation + c.Material.Reputation / 3;
        if (res < 50)
            return -(int)res / 10;
        return (int)res / 10;
    }

    // Пересчитывает нагруженность поставщиков
    private void SupplierRestatement()
    {
        Dictionary<string, int> suppliersCountDictionary = new Dictionary<string, int>();
        foreach(Supplier su in Suppliers)
        {
            su.Quality = su.BaseQuality;
        }

        foreach (Company c in Companies)
        {
            if (!suppliersCountDictionary.ContainsKey(c.Fueller.Name))
                suppliersCountDictionary.Add(c.Fueller.Name, 1);
            else
                suppliersCountDictionary[c.Fueller.Name]+=1;

            if (!suppliersCountDictionary.ContainsKey(c.Transport.Name))
                suppliersCountDictionary.Add(c.Transport.Name, 1);
            else
                suppliersCountDictionary[c.Transport.Name]+=1;

            if (!suppliersCountDictionary.ContainsKey(c.Material.Name))
                suppliersCountDictionary.Add(c.Material.Name, 1);
            else
                suppliersCountDictionary[c.Material.Name]+=1;
        }

        foreach(Supplier s in Suppliers)
        {
            if (suppliersCountDictionary.ContainsKey(s.Name) && suppliersCountDictionary[s.Name] > 1)
                s.Quality -= s.Quality * DownQualityPercent / HundredPercent * (suppliersCountDictionary[s.Name] - 1);
        }
        suppliersCountDictionary.Clear();
    }
    public int[] CalculationPoints()
    {
        int[] money=new int[CountPlayer];
        for(int i=0; i<CountPlayer; i++)
        {
            money[i] = Companies[i].Bank + Companies[i].Personal * 30 + Companies[i].Reputation * 100;
        }
        return money;
    }
}