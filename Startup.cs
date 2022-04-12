using System.Text;
using Age.Data;
using Age.Helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
 

namespace Age
{
    public class Startup
    {
        private string connRoot { get; }
        readonly string sAllowS = "CorsDomain";
        public const string CookieScheme = "SES";
        private string issuer { get; }
        private string signKey { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            // Echo:取得連線資訊
            var connRoot1 = UStore.GetUStore(Configuration["ConnectionStrings:Root"], "Root");
            connRoot = SecurityModule.Decrypt(connRoot1);

            issuer = UStore.GetUStore(Configuration["JwtSettings:Issuer"], "Issuer");
            signKey = UStore.GetUStore(Configuration["JwtSettings:SignKey"], "SignKey");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Echo : DI
            services.AddDbContext<AgeContext>(delegate (DbContextOptionsBuilder options)
            {
                options.UseSqlServer(connRoot);
            });
            services.AddScoped<IAgeServices, AgeServices>();
            // services.AddScoped<IVPDateService, VPDateService>();
            // services.AddScoped<ICovid19Service, Covid19Service>();
            // services.AddScoped<INHIQP701Service, NHIQP701Service>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //* get IP */
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            //** 加入 appsettings.json 自訂的參數取得 */
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IConfigureOptions<CookieAuthenticationOptions>, ConfigureCookie>();
            services.AddSingleton<JwtHelpers>();
            /// * for Cors Domain , 注意網址結尾不要/符號
            services.AddCors(options =>
            {
                options.AddPolicy(name: sAllowS,
                    builder =>
                    {
                        builder.WithOrigins(
                            "https://localhost:5001",
                            "http://localhost:4200",
                            "http://localhost:4200/#",
                            "https://www.country.org.tw"
                        ).AllowAnyHeader().AllowAnyMethod();
                        // 這兩段是回傳資料用的.沒設定的話 Client 收到都是empty array
                    });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                // 當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因
                options.IncludeErrorDetails = true; // 預設值為 true，有時會特別關閉

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // 透過這項宣告，就可以從 "sub" 取值並設定給 User.Identity.Name
                    NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                    // 透過這項宣告，就可以從 "roles" 取值，並可讓 [Authorize] 判斷角色
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

                    // 一般我們都會驗證 Issuer
                    ValidateIssuer = true,
                    ValidIssuer = issuer,

                    // 通常不太需要驗證 Audience
                    ValidateAudience = false,
                    //ValidAudience = "JwtAuthDemo", // 不驗證就不需要填寫

                    // 一般我們都會驗證 Token 的有效期間
                    ValidateLifetime = true,


                    // 如果 Token 中包含 key 才需要驗證，一般都只有簽章而已
                    ValidateIssuerSigningKey = false,

                    // "1234567890123456" 應該從 IConfiguration 取得
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey))
                };
            });



            services.Configure<CookiePolicyOptions>(options =>
            {
                //* 原本是true,但有些人的電腦似乎會無效,所以改成false */
                options.CheckConsentNeeded = context => false;
                // options.MinimumSameSitePolicy = SameSiteMode.Strict;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;

            });

            // * 重要!! 3.1版須添加此段才可以讀取 Request.Body
            // * for Kestrel , ex MAC OS
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // * for IIS
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Strict;
            });


            // services.AddControllersWithViews();
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                };
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
            // app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors(sAllowS); // UseCors 要在 驗證授權 前面
            app.UseAuthentication(); // 先驗證
            app.UseAuthorization(); // 再授權

            // app.Run(async (context) =>
            //    {
            //        var client = new GetVaccLstDataAsync();
            //        var response = await client.GetVaccLstDataAsync();
            //        await context.Response.WriteAsync(response);
            //    });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
