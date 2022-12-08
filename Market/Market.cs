namespace Market_Rules;

public class Market
{
    const int CountTurn = 13;
    const int TimeTurn = 10000;
    const int HundredPercent = 100;
    const int DownQualityPercent = 15;

    private int FondMoney = 1000;
    private object locker = new();

    public int Id { get; private set; }
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
    public delegate void StartTurnHandler(int idSender, New n, int thisTurnTime);
    public event StartTurnHandler StartTurnEvent;

    // конце хода, передается id игры, у которой состояние изменилось
    // новость, произошедшая в начале хода и время этого хода
    public delegate void EndTurnHandler();
    public event EndTurnHandler EndTurnEvent;

    public Market(int id, Company[] companies, List<New> news, 
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
        Дальше ходы должны идти сами,
        действия игроков - будут осуществляться сервером, тупо вручную
        обращаясь к экземпляру Market. 
        Задача Market - играть ходы, выдавать оповещения - все.
        Conditions берутся из new и operation возвратом
    */
    // Главный цикл игры, запускается асинхронно, чтобы мы могли рулить
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
                    EndTurnEvent();
            }
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
                c.Personal * c.Reputation) * FondMoney / HundredPercent;
            c.Profit = c.Sales - c.Personal * 3;
            c.Personal += 1;
            c.Reputation += c.Personal * 3 + CalcReputationSuppl(c);
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
        if (c.Bank > s.Cost)
        {
            Condition operationCond = s.effect.Invoke(Companies, c);
            if (operationCond is not null)
                Conditions.Add(operationCond);
            c.DoneOperations.Add(s);
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
        int maxRep, minRep;
        int fuelR = c.Fueller.Repuation;
        int tranR = c.Transport.Repuation;
        int materR = c.Material.Repuation;

        if (fuelR > tranR)
            if (fuelR > materR)
            {
                maxRep = fuelR;
                if (tranR > materR)
                    minRep = materR;
                else { minRep = tranR; }
            }
            else
            {
                maxRep = materR;
                minRep = tranR;
            }
        else
        {
            if (tranR > materR)
            {
                maxRep = tranR;
                if (fuelR > materR)
                    minRep = materR;
                else { minRep = fuelR; }
            }
            else
            {
                maxRep = materR;
                minRep = fuelR;
            }
        }

        return (maxRep + minRep) / 10;
    }
    // Пересчитывает нагруженность поставщиков
    private void SupplierRestatement()
    {
        Dictionary<string, int> suppliersCountDictionary = new Dictionary<string, int>();

        foreach (Company c in Companies)
        {
            if (!suppliersCountDictionary.ContainsKey(c.Fueller.Name))
                suppliersCountDictionary.Add(c.Fueller.Name, 1);
            else
                suppliersCountDictionary[c.Fueller.Name]++;

            if (!suppliersCountDictionary.ContainsKey(c.Transport.Name))
                suppliersCountDictionary.Add(c.Transport.Name, 1);
            else
                suppliersCountDictionary[c.Transport.Name]++;

            if (!suppliersCountDictionary.ContainsKey(c.Material.Name))
                suppliersCountDictionary.Add(c.Material.Name, 1);
            else
                suppliersCountDictionary[c.Material.Name]++;
        }

        foreach(Supplier s in Suppliers)
        {
            if (suppliersCountDictionary.ContainsKey(s.Name) && suppliersCountDictionary[s.Name] > 1)
                s.Quality -= s.Quality * DownQualityPercent / HundredPercent * (suppliersCountDictionary[s.Name] - 1);
        }
    }
}