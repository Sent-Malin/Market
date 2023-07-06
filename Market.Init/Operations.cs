using Market_Rules;

namespace Market_Init
{
    static class Operations
    {
        //Управление персоналом
        public static Operation OpenCourseOperation()
        {
            Condition OpenCourse(Company[] companies, Company company)
            {
                Condition condition = new Condition();
                condition.CountTurn = 2;
                void GiveFive(Company[] companies)
                {
                    company.Personal += 5;
                }
                condition.endEffect = GiveFive;
                return condition;
            }
            Operation oCOperaion = new Operation("Открыть курсы", "Дает 5 очков персонала через 2 месяца", 200, OpenCourse, "OpCu");
            return oCOperaion;
        }
        
        public static Operation HireContractorsOperation()
        {
            Condition HireContractors(Company[] companies, Company company)
            {
                Condition condition = new Condition();
                company.Personal += 5;
                condition.CountTurn = 2;
                void TakeFive(Company[] companies)
                {
                    company.Personal -= 5;
                }
                condition.endEffect = TakeFive;
                return condition;
            }
            Operation oCOperaion = new Operation("Нанять подряд", "Дает 5 очков персонала на 2 месяца", 200, HireContractors,"HiPe");
            return oCOperaion;
        }

        public static Operation OrginizeMerOperation()
        {
            Condition OrginizeMer(Company[] companies, Company company)
            {
                Condition condition = new Condition();
                company.Personal -= 3;
                company.Sales += 120;
                return condition;
            }
            Operation oCOperaion = new Operation("Организационные меры", "Повышает продажи, немного снижая мотивацию пероснала", 400, OrginizeMer, "OrMe");
            return oCOperaion;
        }

        public static Operation EconomicMerOperation()
        {
            Condition EconomicMer(Company[] companies, Company company)
            {
                Condition condition = new Condition();
                company.Personal += 3;
                company.Sales += 120;
                return condition;
            }
            Operation oCOperaion = new Operation("Экономические меры", "Повышает продажи и мотивацию персонала", 700, EconomicMer, "EcMe");
            return oCOperaion;
        }

        public static Operation SocialMerOperation()
        {
            Condition SocialMer(Company[] companies, Company company)
            {
                Condition condition = new Condition();
                company.Personal += 1;
                company.Sales += 30;
                return condition;
            }
            Operation oCOperaion = new Operation("Социальные меры", "Немного повышает продажи и мотивацию персонала", 200, SocialMer, "SoMe");
            return oCOperaion;
        }

        //Финансовая деятельность
        public static Operation GetCreditOperation()
        {
            Condition GetCredit(Company[] companies, Company company)
            {
                Condition condition = new Condition();
                company.Bank += 250;
                condition.CountTurn = 5;
                condition.TemporaryBankChange = -55;
                return condition;
            }
            Operation oCOperaion = new Operation("Взять кредит", "Вы получаете 250 монет, следующие 5 месяцев со счета будут сниматься по 55 монет", 0, GetCredit, "GeCr");
            return oCOperaion;
        }

        public static Operation InvestmentOperation()
        {
            Condition Investment(Company[] companies, Company company)
            {
                Condition condition = new Condition();
                void GiveHun(Company[] companies)
                {
                    company.Bank += 100;
                }
                condition.endEffect = GiveHun;
                condition.CountTurn = 6;
                condition.TemporaryBankChange = 40;
                return condition;
            }
            Operation oCOperaion = new Operation("Инвестировать", "Вы получаете по 40 монет следующие 6 месяцев и 100 монет по окончании", 300, Investment, "Inve");
            return oCOperaion;
        }
        //Маркетинг
        public static Operation ReklamaOperation()
        {
            Condition Reklama(Company[] companies, Company company)
            {
                Condition condition = new Condition();
                void GiveHun(Company[] companies)
                {
                    company.Bank += 100;
                }
                condition.endEffect = GiveHun;
                condition.CountTurn = 6;
                condition.TemporaryBankChange = 40;
                return condition;
            }
            Operation oCOperaion = new Operation("Реклама", "Повышает продажи компании", 300, Reklama, "Rekl");
            return oCOperaion;
        }
    }
}
