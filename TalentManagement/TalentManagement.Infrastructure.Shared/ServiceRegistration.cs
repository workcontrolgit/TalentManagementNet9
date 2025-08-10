using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TalentManagement.Application.Interfaces;
using TalentManagement.Application.Interfaces.External;
using TalentManagement.Infrastructure.Shared.Services;
using TalentManagement.Infrastructure.Shared.Services.External;
using TalentManagement.Infrastructure.Shared.Configuration;
using StackExchange.Redis;

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
            services.AddHttpClient<USAJobsService>((serviceProvider, client) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var baseUrl = configuration.GetValue<string>("USAJobs:BaseUrl", "https://data.usajobs.gov/api");
                var apiKey = configuration["USAJobs:ApiKey"];
                var userAgent = configuration.GetValue<string>("USAJobs:UserAgent", "TalentManagement/1.0");

                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                
                if (!string.IsNullOrEmpty(apiKey))
                {
                    client.DefaultRequestHeaders.Add("Authorization-Key", apiKey);
                }
                
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            
            // Configure caching
            var cacheConfig = _config.GetSection(CacheConfiguration.SectionName).Get<CacheConfiguration>() ?? new CacheConfiguration();
            services.Configure<CacheConfiguration>(_config.GetSection(CacheConfiguration.SectionName));

            if (cacheConfig.Provider == CacheProvider.Redis)
            {
                if (string.IsNullOrEmpty(cacheConfig.ConnectionString))
                    throw new InvalidOperationException("Redis connection string is required when using Redis cache provider");

                services.AddSingleton<IConnectionMultiplexer>(provider =>
                {
                    return ConnectionMultiplexer.Connect(cacheConfig.ConnectionString);
                });
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = cacheConfig.ConnectionString;
                });
                services.AddSingleton<ICacheService, RedisCacheService>();
            }
            else
            {
                services.AddMemoryCache();
                services.AddSingleton<ICacheService, MemoryCacheService>();
            }
            
            // Register the cached wrapper (USAJobsService is automatically registered by AddHttpClient)
            services.AddScoped<IUSAJobsService, CachedUSAJobsService>();
            
            // USAJobs Code List service - configure HttpClient with basic settings only
            services.AddHttpClient<USAJobsCodeListService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
            });
            services.AddScoped<IUSAJobsCodeListService, USAJobsCodeListService>();
        }
    }
}