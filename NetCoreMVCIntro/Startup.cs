using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCoreMVCIntro.Middlewares;
using System;
using System.Collections.Generic;
using System.IO;
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


    // delegate ayný imzaya sahip methodlarý invoke eden çalýþtýran yapýlar.
    // method elçisi
    // interface gibi yazýlýrlar sadece imzasýný atýyoruz.
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
            services.AddRazorPages();
            services.AddControllers();
            // IOC iþlemlerini ise ConfigureServices kýsmýnda yapýyoruz.
            services.AddTransient<MyMiddleware>(); // IOC ile her istekte bu middleware sýnýfýnýn instance al
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

        // next ile bir sonraki middleware süreci aktarmayan middleware
        private Task MyMiddleware(HttpContext context)
        {
            return context.Response.WriteAsync("Hello World! ");
        }

        // next methodu ile süreci diðer middleware aktaran middleware
        private async Task NextMiddleware1(HttpContext context, Func<Task> next)
        {
             await next();

        }

        private async Task NextMiddleware2(HttpContext context, Func<Task> next)
        {
             await next();
        }

        // public static IApplicationBuilder Use(this IApplicationBuilder app, Func<HttpContext, Func<Task>, Task> middleware);

        // RUN middleware
        // Use middleware
        // Map middelware
        // UseWhen middleware (Çalýþma zamanýnda ek bir servisi sürece dahil etmek için) // alttaki middleware de çalýþtýrýr.
        // MapWhen (Çalýþma zamanýnda bir karar verip ekstra bir özelliði çalýþtýrmak için) altýndaki middleware çalýþmaz.
        // CustomMiddleware ile kendi middleware yaptýk.

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCustomMiddleware();

            //app.UseWelcomePage();



            // bir kodu run middleware kullanarak çalýþtýrýsak orada kod kesilmiþ ve diðer middleware çalýþþtýrýlmamýþ olur.
            // bu middleware kullanýrken dikkat edelim.
            // sunucuya atýnlan her bir request de çalýþýr ve bu middlewareler içerisinde kontroller yapabiliriz.
            // En alt middleware olarak kullanýlmasý daha mantýklý
            //app.Run(async (context) =>
            //{

            //    //if(HttpMethods.IsGet(context.Request.Method))

            //    // response contentType default text/html olarak ayarlýdýr. fakat contentType deðiþtirebiliriz.
            //    context.Response.ContentType = "text/plain";
            //    //context.Response.Redirect("/unauthorized");
            //    await context.Response.WriteAsync("<h1>Hello World!</h1>");

            //});

            // bu kod her bir istekde devreye girip belirli kontroller yapabilmemizi saðlar.
            //app.Use(async (context, next) =>
            //{
            //    // bir sonraki middleware yoluna devam etsin.
            //    await next();

            //});

            //app.Use(async (context, next) =>
            //{
            //    //var @delegate = new RequestDelegate(MyMiddleware);
            //    //await @delegate.Invoke(context);




            //    await next();
            //});

            // gelen admin sayfalarýna giderken authentication middleware sürece dahil et dedik.
            // bir route yakalama iþlemi yapýyoruz. sayfaya gelen istek route göre bir iþlem yapmamýzý saðlar.
            // böyle bir linke gelindiyse çalýþýyor.
            //app.Map("/admin", app =>
            // {
                

            //     app.Use(async (context, next) =>
            //     {

            //         if (context.Request.Query["x-token"] == "cfesdf")
            //         {
            //             await next();
            //         }

            //     });

            //     app.UseAuthentication();
            // });

            // context üzerinden gelen herhangi bir bilgiye göre koþul koyup iþlem yapabilmemizi saðlýyor.
            // default olarak next middleware çalýþýr ve süreci bir sonraki middleware býrakýr.
            //app.UseWhen(context => context.Request.Form["name"] == "ali", config =>
            //{
            //    config.UseDeveloperExceptionPage();
            //});


            // bu kontrol sonrasý uygulamanýn diðer middlewareleri çalýþmaz fakat bu middleware kadar olan kýsým çalýþýr. kendisinden sonrakileri keser. kýsa devre yapar. short cut middleware
            //app.MapWhen(context => context.Request.Form["name"] == "ahmet", config =>
            //{
            //    config.Use(async (context, next) =>
            //    {
            //        context.Response.Redirect("/unauthorized");
            //    });
            //});


            //app.Use(async (context, next) =>
            //{
            //    await next();
            //});

            // name alanýný querystring üzerinden gönderince bu kýsma düþeceðiz.
            // MapWhen lambda ile bir true false kontrolü yapýp eðer durum true ise bu durumda bir iþlemin gerçekleþmesini saðlarýz.
            // MapWhen lambda ile bir true false kontrolü yapýp eðer durum true ise bu durumda bir iþlemin gerçekleþmesini saðlarýz.
            //app.MapWhen(context => context.Request.Query.ContainsKey("name"),
            //                  application => {



            //                      application.Run(async context =>
            //                      {
            //                          await context.Response.WriteAsync("name parametresi yakalandý");
            //                      });

            //                  });


            // MapWhen ile UseWhen middleware arasýndaki fark mapWhen iþleminden sonra next ile diðer middleware çalýþtýrmak için yönlensek dahi mapWhen altýndaki hiç bir middleware istek mapWhen kodunu gerçekleþtirdikten sonra çalýþmayacaktýr. UseWhen gelen istekdeki deðerlere eþleþtiði takdirde diðer middleware ile birlikte ekstra baþka bir middleware request pipeline hattýna girmesini saðlayacak. Diðer middlewarelerin çalýþmasýna engel olmayacaktýr. BNu sebepten dolayý MapWhen kullanýrken iyi düþünmek lazým. Fakat dinamik durumlara karþý araya bazý servislerin eklenmesi istiyorsak yani uygulama farklý durumalara göre bazý yetenekleri uygulama çalýþýrken kazansýn isgtersek UseWhen kullanýlabilir. 
            //app.Use(NextMiddleware1);


            // Test => /Home/Privacy?name ile çalýþýr

            //app.MapWhen(context => context.Request.Query.ContainsKey("name"),
            //                 application =>
            //                 {




            //                     application.Use(async (context, next) =>
            //                     {
            //                         await context.Response.WriteAsync("name parametresi yakalandý");
            //                         await next();
            //                     });

            //                 });


            //app.UseWhen(context => context.Request.Query.ContainsKey("branch"), app => { 


            //});

            // Kendi yazdýðýmýz use middlewatrelerimiz
            //app.Use(NextMiddleware1);
            //app.Use(NextMiddleware2);




            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseStaticFiles();



            ////app.UseWebSockets();
            ////app.UseAuthentication();
            ////app.UseSession();
            //app.UseAuthorization();

         
            app.UseEndpoints(endpoints =>
            {
                // uygulama içerisindeki istekler ya razorpages yönlendirebilir
                // api controller yönlendirebilir
                // mvc controller yönlendirebilir
                // socket için huba yönlendir vs
                // areas yönlendirebilir.


                //endpoints.MapHub();

                //endpoints.MapRazorPages();
                //endpoints.MapControllers();

                //endpoints.MapAreaControllerRoute(name: "admin",
                //    areaName:"admin",
                //    pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");

                // net core mvc app
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Index}/{id?}");


                endpoints.MapGet("/home", async context =>
                {
                    context.Response.ContentType = "text/html";
                    string template = "<h1>Merhaba</h1>";
                    await context.Response.WriteAsync(template);
                });


                endpoints.MapGet("/get-user", async context =>
                {
                    var user = new
                    {
                        UserName = "Ali",
                        Email = "test@test.com"
                    };

                    context.Response.ContentType = "application/json";
                   
                    await context.Response.WriteAsJsonAsync(user);
                });

                endpoints.MapPost("/user-create", async context =>
                {

                    using (StreamReader sm = new StreamReader(context.Request.Body))
                    {
                        string body =  await sm.ReadToEndAsync(); // jsonstring

                        if(context.Request.ContentType == "application/json")
                        {
                            var data = System.Text.Json.JsonSerializer.Deserialize<User>(body);
                        }
                        else if(context.Request.ContentType == "x-www-form-urlencoded")
                        {
                            // var data = bodyParser.Parse<User>(body); Category 
                        }
                    }

                });


            });
        }

        public class User
        {
            public string UserName { get; set; }
            public string Email { get; set; }

        }
    }


}
