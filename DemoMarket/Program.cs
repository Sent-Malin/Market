using DemoMarket;

DemoStart ds = new DemoStart(DemoStart.DefaultMarketInit());
ds.market.Game();
while (ds.market.CurrentTurn != 13)
{
    Thread.Sleep(1000);
    ds.takeActionPlayer();
    while (ds.flag != true) { }
    ds.flag = false;
}
