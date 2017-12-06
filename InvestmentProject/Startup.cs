using InvestmentProject.Data;
using InvestmentProject.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace InvestmentProject
{
    public class Startup
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<InvestmentContext>(options => options.UseSqlServer(Configuration.GetConnectionString("InvestmentConnection")));
            services.AddTransient<ICapitalService, CapitalService>();
            services.AddTransient<IInvestmentContext, InvestmentContext>();
        }

        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env,
            InvestmentContext context)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            new InvestmentDatabase().Initialize(context);
            app.UseMvc();
        }
    }
}
