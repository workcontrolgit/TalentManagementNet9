using TalentManagement.Application.Interfaces;
using TalentManagement.Application.Interfaces.External;
using TalentManagement.Infrastructure.Shared.Services;
using TalentManagement.Infrastructure.Shared.Services.External;

namespace TalentManagement.Infrastructure.Shared
{
    public static class ServiceRegistration
    {
        public static void AddSharedInfrastructure(this IServiceCollection services, IConfiguration _config)
        {
            services.Configure<MailSettings>(_config.GetSection("MailSettings"));
            services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IMockService, MockService>();
            
            // External API services
            services.AddHttpClient<USAJobsService>(client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "TalentManagement/1.0");
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            
            // Caching service
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            
            // Register the base service and the cached wrapper
            services.AddScoped<USAJobsService>();
            services.AddScoped<IUSAJobsService, CachedUSAJobsService>();
        }
    }
}