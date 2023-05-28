using AutoMapper;
using Dominio.EntidadesDto;
using Dominio.Mapa;
using Entidades.Entidades;
using Infraestructura.AccesoDatos;
using Infraestructura.IoC;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rotativa.AspNetCore;
using System;

namespace SaveDoc_v2
{
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            #region//CONFIGURACIÓN DE CONEXION A LA BASE DE DATOS
            services.AddDbContext<DocumentoContext>(options =>            {
                options.UseSqlServer(Configuration.GetConnectionString("SaveDocCore"));
                options.EnableSensitiveDataLogging(true);
            });
            #endregion

            #region//CONFIGURACION DE USO DE IDENTITY - AUTENTICACION DE USUARIO 
            services.AddIdentity<UsuarioApp, RolApp>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 5;
                options.Password.RequiredUniqueChars = 1;

                // Default SignIn settings.
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 4;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = false;

            })
            .AddEntityFrameworkStores<DocumentoContext>()
            .AddDefaultTokenProviders();

            //Declaracion de Polices
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RoleAdmin",
                    policy => policy.RequireRole("ADMIN"));
            });

            #endregion

            #region//CONFIGURACION DE LAS RUTAS ACCESO POR DEFECTO
            services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme,
            opt =>
            {
                opt.LoginPath = "/Account/Index";
                opt.AccessDeniedPath = "/Account/AccesoDenegado";
                opt.Cookie.HttpOnly = true;
                opt.ExpireTimeSpan = TimeSpan.FromMinutes(30);

            });
            #endregion

            #region//CONFIGURACION DEL AUTOMAPPER
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
            #endregion

            #region//CODIGO PARA CAMBIAR EL TAMAÑO DEL BUFFER DE LOS ARCHIVOS
            services.Configure<FormOptions>(x =>
            {
                x.MultipartBodyLengthLimit = 209715200;
            });
            #endregion

            services.Configure<SmtpSettingsDto>(Configuration.GetSection("SmtpSettingsPrueba"));
            ServicioIoC.Service(services);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            RotativaConfiguration.Setup(env.ContentRootPath, "wwwroot/Rotativa");
        }
    }
}
