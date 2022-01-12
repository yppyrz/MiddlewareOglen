using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreMVCIntro
{
    // IServiceProvider interface ile uygulama içerisinde kullanýlan servisler yani uygulamanýn instance almasý gereken servislerin uygulama tanýtýlmasýný bu interface üzerinden saðlýyoruz.
    // ISession ile web sunucu üzerinde oturum bilgileri saklanýyor.
    // Web Client Web Server request attýðý an itibari ile web server üzerinde o web clienta ait bir session oturum açýlýr ve o request için bir sessionID üretilir. Kiþi ayný browserdan tarayýcýdan yaptýktýðý istekler için bu SessionID deðiþmez. Taki End User Browser kapatýp uygulama ile iletiþimi kesene kadar.
    // Kullanýcýya ait hasas bilgiler Cookie üzerinde saklanacaðýna daha gücenli olan ve sunucu tarafýnda saklanan Sessionda tutuluyor. State Management (Durum Yönetimi) ServerSide yönetiminde kullanýlan tekniklerden biriside Session'dýr. 

    // Web Client ile Web Server arasýnda veri paylaþýmýný State Management ile yapýyoruz. Serverside tarafta Application yani uygulama bazlý durum yönetimi yapabiliriz. Örn: Aktif ziyaretçi sayýsý, Session ile de web client olarak web server'a request atan kullanýcýya ait oturum bilgilerini, her bir oturum açan client bazlý tutabiliriz.

    // Client Side tarafta ise Cookie, SessionStorage, LocalStorage, HiddenInput ve QueryString gibi yöntemler ile durum yönetimi yapabiliriz. Cookies genelde authenticated olan kullanýcýlarýn bazý hassas olmayan bilgilerinin tarayýcý tutulup, depolanýp her bir web request de sunucuya iletilmesidir. Bu sayede sunucu oturum açan hesap hakkýnda bilgi edinmiþ olur.


    public delegate Task RequestDelegate(HttpContext context);
    // net core ortamýnda gelen isteklerin execute edilmesini çalýþtýrýlmasýný bu delegate saðlar. yani gelen istekleri async olarak yakalar, içerisinde HttpContext ile web uygulamasýna ait tüm nesneleri barýndýrýr. HttpContext içerisinde temelde iki önemli Nesne bulunmaktadýr. Request diðeri ise Response.  Request Client taraftan web sayfasýna gelen isteði sunucu (web serverda) yakalamamýzý saðlar. Response ise web sunucusunun Web Client'a nasýl bir istek döndüreleceði ile ilgilenir. Html Response, Json Response, Text Response gibi farklý tipte Responselar suncuya döndürülebilir. Session kullanýcýya ait Sunucu tarafýndaki oturum bilgilerini HttpContext barýndýrabiliriz. Oturum açan kullanýcýya ait bilgiler User bilgilerini sunucuda saklayabiliriz. 

    // RequestDelegate Request Sevk edici. Gelen isteðin yönlendirip bir eylemin çalýþmasýný saðlayan elçi.
    // Delegate methodlarýn ayný imzada çalýþmasýný saðlar. Bu sebep ile bir web request geldiði an itibari ile Async yani Task tipinde sonuç döndürmeyen ve içerisinde parametre olarak HttpContext barýndýran herhangi bir methodu çalýþtýrabilir.


    public class Startup
    {
      

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

        public async Task MyFunc(HttpContext context)
        {
            await Task.FromResult("OK");
        }

        public string MyFunc2(HttpContext context)
        {
            return "OK";
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        
          

            // Middleware ara yazýlým. Uygulamaya yapýlan isteklerde isteðin sonladýrmadan önce araya girip uygulamaya yeni bir davranýþý çalýþma zamanýnda ekleme iþlemi. 

            // run dan sonra bir middleware varsa bu middle next methoduna sahip olmadýðý için yani sonlandýrýcý bir middleware olduðu için baþka hiç bir kod çalýþtýrmaz.
            app.Run(async (context) =>
            {
                // JS callback benzeri bir yazým söz konusudur.
                // IApplicationBuilder extention yazýp, RequestDelegate ile bir action tetikleyecek bir mekanizma kurup, bu mekanizma üzerinden bir eylemi yaparak Request pipeline bir özellik kazandýrmýþ bunada Middleware demiþ.


                // delegate ile ayný imza bir method tanýmlayýp, delegate iþi devrettik. delegate de bu methodun çalýþmasýný saðladý.
                //var d = new RequestDelegate(MyFunc);
                //await d.Invoke(context); // delegate üzerinden method çalýþtýrmýþ olduk.


                await context.Response.WriteAsync("Hello World!");

            });

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}
            //app.UseHttpsRedirection();
            //app.UseStaticFiles();

            //app.UseRouting();
            ////app.UseWebSockets();
            ////app.UseAuthentication();
            ////app.UseSession();
            //app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");

            //});
        }
    }

  
}
