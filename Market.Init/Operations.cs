using Market_Rules;

namespace Market_Init
{
    static class Operations
    {
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
            Operation oCOperaion = new Operation("Открыть курсы", "Дает 5 очков персонала через 2 месяца", 200, OpenCourse);
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
            Operation oCOperaion = new Operation("Нанять подряд", "Дает 5 очков персонала на 2 месяца", 200, HireContractors);
            return oCOperaion;
        }
    }
}
