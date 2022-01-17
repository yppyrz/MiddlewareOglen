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
using System.Web;

namespace NetCoreMVCIntro
{
    // IServiceProvider interface ile uygulama i�erisinde kullan�lan servisler yani uygulaman�n instance almas� gereken servislerin uygulama tan�t�lmas�n� bu interface �zerinden sa�l�yoruz.
    // ISession ile web sunucu �zerinde oturum bilgileri saklan�yor.
    // Web Client Web Server request att��� an itibari ile web server �zerinde o web clienta ait bir session oturum a��l�r ve o request i�in bir sessionID �retilir. Ki�i ayn� browserdan taray�c�dan yapt�kt��� istekler i�in bu SessionID de�i�mez. Taki End User Browser kapat�p uygulama ile ileti�imi kesene kadar.
    // Kullan�c�ya ait hasas bilgiler Cookie �zerinde saklanaca��na daha g�cenli olan ve sunucu taraf�nda saklanan Sessionda tutuluyor. State Management (Durum Y�netimi) ServerSide y�netiminde kullan�lan tekniklerden biriside Session'd�r. 

    // Web Client ile Web Server aras�nda veri payla��m�n� State Management ile yap�yoruz. Serverside tarafta Application yani uygulama bazl� durum y�netimi yapabiliriz. �rn: Aktif ziyaret�i say�s�, Session ile de web client olarak web server'a request atan kullan�c�ya ait oturum bilgilerini, her bir oturum a�an client bazl� tutabiliriz.

    // Client Side tarafta ise Cookie, SessionStorage, LocalStorage, HiddenInput ve QueryString gibi y�ntemler ile durum y�netimi yapabiliriz. Cookies genelde authenticated olan kullan�c�lar�n baz� hassas olmayan bilgilerinin taray�c� tutulup, depolan�p her bir web request de sunucuya iletilmesidir. Bu sayede sunucu oturum a�an hesap hakk�nda bilgi edinmi� olur.


    // delegate ayn� imzaya sahip methodlar� invoke eden �al��t�ran yap�lar.
    // method el�isi
    // interface gibi yaz�l�rlar sadece imzas�n� at�yoruz.
    public delegate Task RequestDelegate(HttpContext context);
  
    // net core ortam�nda gelen isteklerin execute edilmesini �al��t�r�lmas�n� bu delegate sa�lar. yani gelen istekleri async olarak yakalar, i�erisinde HttpContext ile web uygulamas�na ait t�m nesneleri bar�nd�r�r. HttpContext i�erisinde temelde iki �nemli Nesne bulunmaktad�r. Request di�eri ise Response.  Request Client taraftan web sayfas�na gelen iste�i sunucu (web serverda) yakalamam�z� sa�lar. Response ise web sunucusunun Web Client'a nas�l bir istek d�nd�relece�i ile ilgilenir. Html Response, Json Response, Text Response gibi farkl� tipte Responselar suncuya d�nd�r�lebilir. Session kullan�c�ya ait Sunucu taraf�ndaki oturum bilgilerini HttpContext bar�nd�rabiliriz. Oturum a�an kullan�c�ya ait bilgiler User bilgilerini sunucuda saklayabiliriz. 

    // RequestDelegate Request Sevk edici. Gelen iste�in y�nlendirip bir eylemin �al��mas�n� sa�layan el�i.
    // Delegate methodlar�n ayn� imzada �al��mas�n� sa�lar. Bu sebep ile bir web request geldi�i an itibari ile Async yani Task tipinde sonu� d�nd�rmeyen ve i�erisinde parametre olarak HttpContext bar�nd�ran herhangi bir methodu �al��t�rabilir.



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
            // IOC i�lemlerini ise ConfigureServices k�sm�nda yap�yoruz.
            services.AddTransient<MyMiddleware>(); // IOC ile her istekte bu middleware s�n�f�n�n instance al
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

        // next ile bir sonraki middleware s�reci aktarmayan middleware
        private Task MyMiddleware(HttpContext context)
        {
            return context.Response.WriteAsync("Hello World! ");
        }

        // next methodu ile s�reci di�er middleware aktaran middleware
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
        // UseWhen middleware (�al��ma zaman�nda ek bir servisi s�rece dahil etmek i�in) // alttaki middleware de �al��t�r�r.
        // MapWhen (�al��ma zaman�nda bir karar verip ekstra bir �zelli�i �al��t�rmak i�in) alt�ndaki middleware �al��maz.
        // CustomMiddleware ile kendi middleware yapt�k.

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCustomMiddleware();

            //app.UseWelcomePage();



            // bir kodu run middleware kullanarak �al��t�r�sak orada kod kesilmi� ve di�er middleware �al���t�r�lmam�� olur.
            // bu middleware kullan�rken dikkat edelim.
            // sunucuya at�nlan her bir request de �al���r ve bu middlewareler i�erisinde kontroller yapabiliriz.
            // En alt middleware olarak kullan�lmas� daha mant�kl�
            //app.Run(async (context) =>
            //{

            //    //if(HttpMethods.IsGet(context.Request.Method))

            //    // response contentType default text/html olarak ayarl�d�r. fakat contentType de�i�tirebiliriz.
            //    context.Response.ContentType = "text/plain";
            //    //context.Response.Redirect("/unauthorized");
            //    await context.Response.WriteAsync("<h1>Hello World!</h1>");

            //});

            // bu kod her bir istekde devreye girip belirli kontroller yapabilmemizi sa�lar.
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

            // gelen admin sayfalar�na giderken authentication middleware s�rece dahil et dedik.
            // bir route yakalama i�lemi yap�yoruz. sayfaya gelen istek route g�re bir i�lem yapmam�z� sa�lar.
            // b�yle bir linke gelindiyse �al���yor.
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

            // context �zerinden gelen herhangi bir bilgiye g�re ko�ul koyup i�lem yapabilmemizi sa�l�yor.
            // default olarak next middleware �al���r ve s�reci bir sonraki middleware b�rak�r.
            //app.UseWhen(context => context.Request.Form["name"] == "ali", config =>
            //{
            //    config.UseDeveloperExceptionPage();
            //});


            // bu kontrol sonras� uygulaman�n di�er middlewareleri �al��maz fakat bu middleware kadar olan k�s�m �al���r. kendisinden sonrakileri keser. k�sa devre yapar. short cut middleware
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

            // name alan�n� querystring �zerinden g�nderince bu k�sma d��ece�iz.
            // MapWhen lambda ile bir true false kontrol� yap�p e�er durum true ise bu durumda bir i�lemin ger�ekle�mesini sa�lar�z.
            // MapWhen lambda ile bir true false kontrol� yap�p e�er durum true ise bu durumda bir i�lemin ger�ekle�mesini sa�lar�z.
            //app.MapWhen(context => context.Request.Query.ContainsKey("name"),
            //                  application => {



            //                      application.Run(async context =>
            //                      {
            //                          await context.Response.WriteAsync("name parametresi yakaland�");
            //                      });

            //                  });


            // MapWhen ile UseWhen middleware aras�ndaki fark mapWhen i�leminden sonra next ile di�er middleware �al��t�rmak i�in y�nlensek dahi mapWhen alt�ndaki hi� bir middleware istek mapWhen kodunu ger�ekle�tirdikten sonra �al��mayacakt�r. UseWhen gelen istekdeki de�erlere e�le�ti�i takdirde di�er middleware ile birlikte ekstra ba�ka bir middleware request pipeline hatt�na girmesini sa�layacak. Di�er middlewarelerin �al��mas�na engel olmayacakt�r. BNu sebepten dolay� MapWhen kullan�rken iyi d���nmek laz�m. Fakat dinamik durumlara kar�� araya baz� servislerin eklenmesi istiyorsak yani uygulama farkl� durumalara g�re baz� yetenekleri uygulama �al���rken kazans�n isgtersek UseWhen kullan�labilir. 
            //app.Use(NextMiddleware1);


            // Test => /Home/Privacy?name ile �al���r

            //app.MapWhen(context => context.Request.Query.ContainsKey("name"),
            //                 application =>
            //                 {




            //                     application.Use(async (context, next) =>
            //                     {
            //                         await context.Response.WriteAsync("name parametresi yakaland�");
            //                         await next();
            //                     });

            //                 });


            //app.UseWhen(context => context.Request.Query.ContainsKey("branch"), app => { 


            //});

            // Kendi yazd���m�z use middlewatrelerimiz
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
                // uygulama i�erisindeki istekler ya razorpages y�nlendirebilir
                // api controller y�nlendirebilir
                // mvc controller y�nlendirebilir
                // socket i�in huba y�nlendir vs
                // areas y�nlendirebilir.


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
                            HttpUtility.UrlDecode(body);
                            
                            
                        }
                        else if (context.Request.ContentType == "application/x-www-form-urlencoded")
                        {
                            var decodedData = HttpUtility.UrlDecode(body);

                            NetCoreMVCIntro.BodyParser.BodyParser.Parse<User>(decodedData);

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
