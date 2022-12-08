using Market_Rules;

namespace Market_Init
{
    public class Factory
    {
        public static Market GetMarket(int ID) 
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
                Operations.HireContractorsOperation()
            };
            Supplier mat1 = new Supplier("Ernst Mines", 50, 70, Specialization.Material);
            Supplier mat2 = new Supplier("Eco Recycling", 100, 50, Specialization.Material);
            Supplier mat3 = new Supplier("Jack", 10, 90, Specialization.Material);

            Supplier fuel1 = new Supplier("Werden Gas", 40, 80, Specialization.Fuel);
            Supplier fuel2 = new Supplier("Wrang Methan", 70, 60, Specialization.Fuel);
            Supplier fuel3 = new Supplier("Umbrella Coal", 20, 100, Specialization.Fuel);

            Supplier tran1 = new Supplier("Ernandez River", 70, 30, Specialization.Transportation);
            Supplier tran2 = new Supplier("Taggert Trans", 60, 60, Specialization.Transportation);
            Supplier tran3 = new Supplier("Frank Trucks", 20, 100, Specialization.Transportation);
            List<Supplier> suppliers = new List<Supplier>() { 
                mat1, mat2, mat3, fuel1, fuel2, fuel3, tran1, tran2, tran3 };

            Company company1 = new Company("Squirrel Fabric", 10, 1000, 100, 100, 60, mat1, fuel1, tran1);
            Company company2 = new Company("Terran Production", 10, 1000, 100, 100, 60, mat2, fuel2, tran3);
            Company company3 = new Company("Dream Factory", 10, 1000, 100, 100, 60, mat1, fuel3, tran2);
            Company company4 = new Company("Wayne Enterprises", 10, 1000, 100, 100, 60, mat3, fuel2, tran3);

            Company[] companies = new Company[] { company1, company2, company3, company4, };

            Market m = new Market(ID, companies, news, suppliers, operations, 1000);
            return m;
        }
    }
}