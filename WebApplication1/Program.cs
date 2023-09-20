using System.Net;
using Microsoft.AspNetCore;
using WebApplication1.Hubs;
var builder = WebHost.CreateDefaultBuilder(args);
builder.ConfigureServices((hostContext, services) =>
{
    services.AddControllersWithViews();
    services.AddSignalR();
    services.AddDistributedMemoryCache();
    services.AddSession(c => c.IdleTimeout = TimeSpan.FromMinutes(10));
});

builder.Configure((app) =>
{
    if (!app.ApplicationServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
    }

    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthorization();
    app.UseSession();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        endpoints.MapHub<ChatHub>("/chatHub");
    });
});

builder.UseUrls("http://localhost:5141"); // Specify the desired port here

var app = builder.Build();

app.Run();