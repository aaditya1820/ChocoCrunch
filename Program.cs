using Aaditya.Data;
using Aaditya.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure the database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Configure email service
builder.Services.AddTransient<IEmailSender, EmailSender>();

// 3. Enable session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 4. Add distributed memory cache (required for session storage)
builder.Services.AddDistributedMemoryCache();

// 5. Add controllers with views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 6. Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 7. Enable session before authentication and authorization
app.UseSession();

app.UseRouting();
app.UseAuthorization();

// 8. Default MVC route configuration
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 9. Redirect root URL ("/") to "wwwroot/chocolux-master/index.html"
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/chocolux-master/index.html");
        return;
    }
    await next();
});

app.Run();