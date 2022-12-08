using Market_Web;
using Market_Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<GameHub>("/hub");

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
