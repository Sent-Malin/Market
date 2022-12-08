using Market_Rules;
using System.Runtime.InteropServices;

namespace DemoMarket
{
    public class DemoStart
    {
        // Функции и константак для прерывания потока ввода консоли
        const int STD_INPUT_HANDLE = -10;

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CancelIoEx(IntPtr handle, IntPtr lpOverlapped);

        public Market market;

        public DemoStart(Market market)
        {
            this.market = market;
            // Подписываем DemoStart на событие - начало хода в игре
            market.StartTurnEvent += DemoShowTurn;
            // Подписываем DemoStart на событие - конец хода в игре
            market.EndTurnEvent += KillConsole;
        }

        // Поле для извращения с потоками, нужно для считывания события и прерывания
        // консольного ввода без await в цикле
        public bool flag = false;

        // Выводит данные компании единственного игрока - тебя), но по идее должен выводить
        // данные всех компаний, т.к. начало хода
        public void DemoShowTurn(int idSender, New n, int thisTurnTime)
        {
            Console.WriteLine("Turn number:" + market.CurrentTurn.ToString());
            Console.WriteLine("Time on this turn:" + thisTurnTime.ToString());
            // Новость оказывает эффект и выводится заголовок новости
            Console.WriteLine(n.Title);
            // Вывод ресурсов твоей компании
            Console.WriteLine(String.Format("Компания: {0}, Персонал: {1}, Банк: {2}, Прибыль: +{3}, Продажи: {4}," +
                "Репутация: {5}, \n Поставщики: материалов - {6}, топлива - {7}, перевозок - {8} \n", market.Companies[0].Name,
                market.Companies[0].Personal.ToString(), market.Companies[0].Bank.ToString(), market.Companies[0].Profit.ToString(),
                market.Companies[0].Sales.ToString(), market.Companies[0].Reputation.ToString(), market.Companies[0].Material.Name,
                market.Companies[0].Fueller.Name, market.Companies[0].Transport.Name));
        }

        public void KillConsole()
        {
            // Прерывание потока ввода консоли для вывода
            // информации о след. ходе(генерирующее исключение)
            try
            {
                var handle = GetStdHandle(STD_INPUT_HANDLE);
                CancelIoEx(handle, IntPtr.Zero);
            }
            catch (InvalidOperationException) { Console.WriteLine("Turn over"); }
            catch (OperationCanceledException) { Console.WriteLine("Turn over"); }
            flag = true;
        }

        public static Market DefaultMarketInit()
        {
            // Initialization
            // Create Suppliers
            Supplier mat = new Supplier("Mater", 90, 90, Specialization.Material);
            Supplier fu = new Supplier("Fuler", 90, 90, Specialization.Fuel);
            Supplier tran = new Supplier("Travis", 90, 90, Specialization.Transportation);
            Supplier wat = new Supplier("Walter", 20, 100, Specialization.Transportation);
            List<Supplier> suppliers = new List<Supplier>() { mat, fu, tran, wat };

            // Create Company
            Company company = new Company("TroyMarsh", 20, 2000, 100, 100, 100, mat, fu, tran);
            Company[] companies = new Company[] { company };

            // Create News
            // Create Effects
            Condition Hunger(Company[] companies) 
            { 
                foreach (Company co in companies) { co.Bank -= 200; }
                Condition c = new Condition();
                c.CompanyUnderEffect.AddRange(companies);
                c.CountTurn = 5;
                c.TemporaryBankChange = -2000;
                return c;
            }
            Condition Grants(Company[] companies) 
            { 
                foreach (Company c in companies) { c.Bank += 120; }

                return null;
            }
            New hungernew = new New("Hunger in country!", "All company -200 money", Hunger);
            New grantsnew = new New("Governments order on the way!", "All company +120 money", Grants);
            List<New> news = new List<New>() { hungernew, grantsnew };

            // Create Operations
            // Create Effects
            static Condition OpenCourse(Company[] companies, Company company) 
            { 
                Condition condition = new Condition();
                condition.TemporaryTimeChange = 10000;
                company.Bank -= 200; 
                company.Personal += 5;

                return condition;
            }
            static Condition StateInspections(Company[] companies, Company company)
            { 
                foreach (Company c in companies) 
                { 
                    if (c.Personal <= 5) 
                    { 
                        c.Bank -= 200; 
                        c.Personal += 5; 
                    } 
                    else c.Bank += 100; 
                }
                Condition cond=new Condition();
                cond.CountTurn = 2;
                cond.TemporaryPersonalChange = 1000;
                cond.CompanyUnderEffect.Add(company);

                return cond;
            }
            Operation oCOperaion = new Operation("Open course", "Spend 200 mone and give 5 personal", 200, OpenCourse);
            Operation sIOperation = new Operation("State inspections", "All companies checked by personal, if it less 5, " +
                "bank low on 200 and give 5 personal, else bank add on 100", 0, StateInspections);
            List<Operation> operations = new List<Operation>() { oCOperaion, sIOperation };

            Market market = new Market(1, companies, news, suppliers, operations, 1000);
           
            return market;
        }

        // Обрабатывает ввод игрока
        public void takeActionPlayer()
        {
            Task consoleHearer = new Task(() =>
            {
                ShowActions();
                while (flag != true)
                {
                    try
                    {
                        // Показывает варианты команд - действий
                        string? action = Console.In.ReadLine();
                        if (action == "list sup")
                            ShowSup();
                        if (action == "list oper")
                            ShowOperation();
                        if (action.IndexOf("oper:") != -1)
                        {
                            int ind = int.Parse(action.Substring(action.IndexOf(" ")));
                            Console.WriteLine(market.Operation(market.Companies[0], market.Operations[ind]));
                        }
                        if (int.TryParse(action, out int r))
                            TurnSwitchSupl(r);
                    }
                    catch (InvalidOperationException) { }
                    catch (OperationCanceledException) { }//Console.WriteLine("Turn over"); }
                }
            });
            consoleHearer.Start();
        }
        public void ShowActions()
        {
            // Выводит данные компании, твоей.
            Console.WriteLine(String.Format("Your suppliers:\n Material - {0}, Fueller - {1}, " +
                                "Transport - {2}.", market.Companies[0].Material.Name, market.Companies[0].Fueller.Name,
                                market.Companies[0].Transport.Name));
            Console.WriteLine("Commands: list sup | list oper | oper: {your index} |" +
                "{just index suppliers to change}\n");
        }
        public void TurnSwitchSupl(int i)
        {
            if(i<market.Suppliers.Count)
            market.ChooseSupplier(market.Companies[0], market.Suppliers[i]);
            else { Console.WriteLine("Wrong number supl");}
        }

        public void ShowSup()
        {
            Console.WriteLine(String.Format("All suppliers:\n"));
            foreach (Supplier s in market.Suppliers)
            {
                Console.Write(market.Suppliers.IndexOf(s) + " - " + s.Name.ToString() + " ");
            }
        }

        public void ShowOperation()
        {
            Console.WriteLine(String.Format("All operations:\n"));
            foreach (Operation s in market.Operations)
            {
                // Исключаем сделанные исследования, их не выводим
                //if (!market.Companies[0].DoneOperations.Contains(s))
                Console.WriteLine(market.Operations.IndexOf(s) + " - " + s.Name + " " + s.Description+"\n");
            }
        }
    }
}
