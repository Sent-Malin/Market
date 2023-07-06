using Market_Web;
using Market_Web.Hubs;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Market.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options=>options.RootDirectory="/Pages");
builder.Services.AddSignalR();
builder.Services.AddSingleton<GameHub>();
builder.Services.AddMvc();
builder.Services.Configure<RazorViewEngineOptions>(o =>
{
    o.ViewLocationFormats.Add("/Pages/{0}"+RazorViewEngine.ViewExtension);
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");
    endpoints.MapHub<GameHub>("/hub");
});

IHostApplicationLifetime applicationLifetime = app.Lifetime;
MarketDbContext db = new MarketDbContext();
applicationLifetime.ApplicationStopped.Register(db.PlayersOffline);

app.Run();
