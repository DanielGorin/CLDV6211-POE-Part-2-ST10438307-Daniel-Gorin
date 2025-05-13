using Microsoft.EntityFrameworkCore;
using CLDV6211_POE_Part_1_ST10438307_Daniel_Gorin.Services;
using CLDV6211_POE_Part_1_ST10438307_Daniel_Gorin.Models;

namespace CLDV6211_POE_Part_1_ST10438307_Daniel_Gorin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //Added in to connect to Azure Db
            builder.Services.AddDbContext<EaseDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefCon")));

            builder.Services.AddSingleton<BlobStorageService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
