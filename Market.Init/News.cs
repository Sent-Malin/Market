using Market_Rules;

namespace Market_Init
{
    static class News
    {
        public static New EconomicGrowthNew()
        {
            Condition EconomicGrowth(Company[] companies)
            {
                foreach (Company co in companies) { co.Sales += 20;  }
                Condition c = new Condition();
                c.CompanyUnderEffect.AddRange(companies);
                c.CountTurn = 3;
                c.TemporaryPersonalChange = 2;
                c.TemporarySalesChange = 20;
                return c;
            }
            New economicGrow = new New("Экономический подъем!", "Продажи всех компаний, как и качество персонала будут расти след. 3 месяца.", EconomicGrowth);
            return economicGrow;
        }

        public static New EconomicRecessionNew()
        {
            Condition EconomicRecession(Company[] companies)
            {
                foreach (Company co in companies) { co.Sales -= 20; }
                Condition c = new Condition();
                c.CompanyUnderEffect.AddRange(companies);
                c.CountTurn = 3;
                c.TemporaryPersonalChange = -2;
                c.TemporarySalesChange = -20;
                return c;
            }
            New economicRecession = new New("Экономический спад!", "Продажи всех компаний, как и качество персонала будут падать след. 3 месяца.", EconomicRecession);
            return economicRecession;
        }

        public static New PlanGovermentCheckNew()
        {
            Condition PlanGovermentCheck(Company[] companies)
            {
                Condition c = new Condition();
                c.CompanyUnderEffect.AddRange(companies);
                c.CountTurn = 4;
                void Check(Company[] companies)
                {
                    foreach(Company c in companies)
                    {
                        if (c.Personal < 10)
                        {
                            c.Bank -= 200;
                        }
                    }
                }
                c.endEffect = Check;
                return c;
            }
            New planGovermentCheck = new New("Плановая проверка!", "Через 4 месяца ожидается проверка условий работы персонала, если" +
                "значение персонала будет меньше 10, будет наложен штраф в 200 монет.", PlanGovermentCheck);
            return planGovermentCheck;
        }

        public static New GovermentAntiMonopolyNew()
        {
            Condition GovermentAntiMonopoly(Company[] companies)
            {
                int min=0, max=0;
                for (int i = 0; i < companies.Length-1; i++)
                {
                    if (companies[i].Sales < companies[min].Sales)
                        min = i;
                    if (companies[i].Sales > companies[max].Sales)
                        max = i;
                }
                companies[min].Bank += 200;
                companies[max].Bank -= 200;
                return null;
            }
            New govermentAntiMonopoly = new New("Антимонопольное регулирование!", "200 монет компании с самыми высокими показателями прибыли" +
                "будут перераспределены в пользу компании с самыми низкими прибылями.", GovermentAntiMonopoly);
            return govermentAntiMonopoly;
        }
        public static New GovermentGrantNew()
        {
            Condition GovermentGrant(Company[] companies)
            {
                int max = 0;
                for (int i = 0; i < companies.Length - 1; i++)
                {
                    if (companies[i].Reputation > companies[max].Sales)
                        max = i;
                }
                companies[max].Bank += 200;
                return null;
            }
            New govermentGrant = new New("Государственный грант!", "200 монет будут выданы компании с самой высокой репутацией.", GovermentGrant);
            return govermentGrant;
        }

    }
}
