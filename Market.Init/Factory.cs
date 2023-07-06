using Market_Rules;

namespace Market_Init
{
    public class Factory
    {
        public static Market GetMarket(string ID) 
        { 
            List<New> news= new List<New>() 
            {
                News.EconomicGrowthNew(),
                News.EconomicRecessionNew(),
                News.PlanGovermentCheckNew(),
                News.GovermentAntiMonopolyNew(),
                News.GovermentGrantNew()
            };
            List<Operation> operations = new List<Operation>()
            {
                Operations.OpenCourseOperation(),
                Operations.HireContractorsOperation(),
                Operations.GetCreditOperation()
            };
            Supplier mat1 = new Supplier("Зеленый Выбор", 100, 50, Specialization.Material);
            Supplier mat2 = new Supplier("Шахты Эрнеста", 50, 70, Specialization.Material);
            Supplier mat3 = new Supplier("Серый импорт", 10, 90, Specialization.Material);

            Supplier fuel1 = new Supplier("Бензин Вердена", 40, 80, Specialization.Fuel);
            Supplier fuel2 = new Supplier("Вранг Медан", 70, 60, Specialization.Fuel);
            Supplier fuel3 = new Supplier("Уголь Umbrella", 20, 100, Specialization.Fuel);

            Supplier tran1 = new Supplier("Речные перевозки Эрнандеса", 70, 30, Specialization.Transportation);
            Supplier tran2 = new Supplier("Поезда Таггерта", 60, 60, Specialization.Transportation);
            Supplier tran3 = new Supplier("Грузовики френка", 20, 100, Specialization.Transportation);
            List<Supplier> suppliers = new List<Supplier>() { 
                mat1, mat2, mat3, fuel1, fuel2, fuel3, tran1, tran2, tran3 };

            Company company1 = new Company("Фабрика белок", 10, 300, 100, 100, 20, mat1, fuel1, tran1);
            Company company2 = new Company("ПромТрон", 10, 300, 100, 100, 20, mat2, fuel2, tran3);
            Company company3 = new Company("Фабрика звезд", 10, 300, 100, 100, 20, mat1, fuel3, tran2);
            Company company4 = new Company("Конструктив", 10, 300, 100, 100, 20, mat3, fuel2, tran3);

            Company[] companies = new Company[] { company1, company2, company3, company4, };

            Market m = new Market(ID, companies, news, suppliers, operations, 1000);
            return m;
        }
    }
}