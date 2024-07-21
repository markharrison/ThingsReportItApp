using ThingsReportIt;

namespace ThingsReportItApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");
            builder.Services.AddRazorPages();
            builder.Services.AddSingleton<AppConfig>(new AppConfig(builder.Configuration));
            builder.Services.AddHttpClient();


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
