using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Seed;
using UIStoreMVC.Validations;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ==========================
        // MVC + Razor + FluentValidation
        // ==========================
        builder.Services
            .AddControllersWithViews()
            .AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssemblyContaining<RegisterViewModelValidator>();
                fv.RegisterValidatorsFromAssemblyContaining<AgencyApplicationCreateViewModelValidator>();
            });

        builder.Services.AddRazorPages();

        // ==========================
        // Cache / Session / HttpContext
        // ==========================
        builder.Services.AddMemoryCache();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSession();

        // ==========================
        // DbContext-Repo-Service Kayıtları
        // ==========================
        builder.Services.AddInfrastructureServices(builder.Configuration);
        builder.Services.AddApplicationServices();

        // ==========================
        // APP BUILD
        // ==========================
        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseSession();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        // ==========================
        // ROUTING
        // ==========================
        app.MapControllerRoute(
       name: "areas",
       pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapRazorPages();

        // ==========================
        // Migration + Seed
        // ==========================
        app.ConfigureAndCheckMigration();
        await app.ConfigureSeedDataAsync();
        await app.ConfigureDefaultAdminUserAsync();

        app.Run();
    }
}
