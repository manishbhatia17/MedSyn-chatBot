using System;
using MedGyn.MedForce.Common.Configurations;
using MedGyn.MedForce.Data.Interfaces;
using MedGyn.MedForce.Data.Repositories;
using MedGyn.MedForce.Facade.Facades;
using MedGyn.MedForce.Facade.Interfaces;
using MedGyn.MedForce.Service.Interfaces;
using MedGyn.MedForce.Service.Services;
using MedGyn.MedForce.Web.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ApplicationInsights.DependencyCollector;
using Medforce.Graph.Services;
using Medgyn.Meforce.LLMAgent.LLMAgents;
using MedGyn.MedForce.Facade.Handlers.Interfaces;
using MedGyn.MedForce.Facade.Handlers;
using MedGyn.MedForce.Facade.Factories.Interfaces;
using MedGyn.MedForce.Facade.Factories;

namespace MedGyn.MedForce.Web
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			//think this is for NHibernate?
			services.AddDistributedMemoryCache();

			services.AddMemoryCache();

			services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromDays(5);
				options.Cookie.IsEssential = true;
			});

			services.AddHttpContextAccessor();
			services.AddHttpClient();

			services.AddMvc(option => option.EnableEndpointRouting = false).AddRazorRuntimeCompilation();

			//facades
			services.AddScoped<IAccountFacade, AccountFacade>();
			services.AddScoped<ICodeFacade, CodeFacade>();
			services.AddScoped<ICustomerFacade, CustomerFacade>();
			services.AddScoped<IChatBotFacade, ChatBotFacade>();
			services.AddScoped<IChatBotService, ChatBotService>();
			services.AddScoped<ICustomerOrderFacade, CustomerOrderFacade>();
			services.AddScoped<IProductFacade, ProductFacade>();
			services.AddScoped<IPurchaseOrderFacade, PurchaseOrderFacade>();
			services.AddScoped<ISecurityFacade, SecurityFacade>();
			services.AddScoped<IStatusFacade, StatusFacade>();
			services.AddScoped<IUserFacade, UserFacade>();
			services.AddScoped<IVendorFacade, VendorFacade>();
			services.AddScoped<IEmailResponseFacade, EmailResponseFacade>();

			//Factories
			services.AddScoped<IEmailBotCommandHandlerFactory, EmailbotCommandHandlerFactory>();

			//CommandHandlers
			services.AddTransient<IEmailBotCommandHandler, GetProductByNameCommandHandler>();
			services.AddTransient<IEmailBotCommandHandler, GetProductByIdCommandHandler>();
			services.AddTransient<IEmailBotCommandHandler, GetCustomerPOCommandHandler>();
			services.AddTransient<IEmailBotCommandHandler, GetCustomerOrderCommandHandler>();

			//services
			services.AddScoped<IAuthenticationService, AuthenticationService>();
			services.AddScoped<IBlobStorageService, BlobStorageService>();
			services.AddScoped<ICodeService, CodeService>();
			services.AddScoped<ICustomerOrderService, CustomerOrderService>();
			services.AddScoped<ICustomerService, CustomerService>();
			services.AddScoped<IDbMigrationService, DbMigrationService>();
			services.AddScoped<IEmailService, Service.Services.EmailService>();
			services.AddScoped<IFedExService, FedExService>();
			services.AddScoped<IProductService, ProductService>();
			services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
			services.AddScoped<IRoleService, RoleService>();
			services.AddScoped<ISecurityService, SecurityService>();
			services.AddSingleton<IShipStationAPIService, ShipStationAPIService>(); //singleton because it's a wrapper for HttpClient
			services.AddScoped<IUPSService, UPSService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IVendorService, VendorService>();
			services.AddScoped<IOrderAnalysisService, AzureOrderAnalysisService>();

			//repositories
			services.AddScoped<ICodeRepository, CodeRepository>();
			services.AddScoped<ICustomerOrderRepository, CustomerOrderRepository>();
			services.AddScoped<ICustomerRepository, CustomerRepository>();
			services.AddScoped<IDbMigrationRepository, DbMigrationRepository>();
			services.AddScoped<IProductRepository, ProductRepository>();
			services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
			services.AddScoped<IRoleRepository, RoleRepository>();
			services.AddScoped<ISecurityRepository, SecurityRepository>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IVendorRepository, VendorRepository>();
			services.AddScoped<IChatBotRepository, ChatBotRepository>();

			services.AddScoped<Medforce.Graph.Services.Interfaces.IEmailService, GraphEmailService>();
			services.AddScoped<ILLMAgent, Medgyn.Meforce.LLMAgent.LLMAgents.ChatGPTAgent>();
			services.AddScoped<Medgyn.Meforce.LLMAgent.Services.ILLMService, Medgyn.Meforce.LLMAgent.Services.ChatGPTLLMService>();
			services.AddScoped<Medforce.Graph.Services.Interfaces.ISharePointListSearchService, GraphSharePointListSearch>();
			services.AddScoped<Medforce.Graph.Services.Interfaces.IGraphWebhookSubscriptions, GraphWebhookSubscriptions>();

			//filters
			services.AddScoped<AuthorizeFilter>();

			//configurations
			services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
			services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
			services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
			services.Configure<ShipStationAPISettings>(Configuration.GetSection("ShipStationAPISettings"));

			services.AddAuthentication().AddCookie(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, options =>
			{
				options.LoginPath = "/Account/Login";
				options.AccessDeniedPath = "/Account/Login";
				options.SlidingExpiration = true;
			});

			services.AddAuthentication(o =>
			{
				o.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
			});

			services.AddAuthorization(options =>
			{
				options.DefaultPolicy = new AuthorizationPolicyBuilder()
				.RequireAuthenticatedUser()
				.AddAuthenticationSchemes(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
				.Build();
			});

			services.AddNHibernate(Configuration.GetConnectionString("DefaultContext"));

			services.AddApplicationInsightsTelemetry();
			services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) => { module.EnableSqlCommandTextInstrumentation = true; });
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseStaticFiles();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseStatusCodePagesWithReExecute("/Error/{0}");
				app.UseHsts();
			}

			app.UseStaticFiles();
			app.UseAuthentication();
			var cookiePolicyOptions = new CookiePolicyOptions
			{
				MinimumSameSitePolicy = SameSiteMode.Strict,
			};
			app.UseCookiePolicy(cookiePolicyOptions);

			app.UseSession();

			var routeBuilder = new RouteBuilder(app);
			app.UseMvcWithDefaultRoute();
			app.UseRouter(routeBuilder.Build());

			app.Use(async (context, next) => { Console.WriteLine($"Incoming: {context.Request.Method} {context.Request.Path}"); await next(); });
		}
	}
}
