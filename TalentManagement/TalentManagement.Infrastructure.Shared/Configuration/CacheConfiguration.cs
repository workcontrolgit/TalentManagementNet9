namespace TalentManagement.Infrastructure.Shared.Configuration
{
    public class CacheConfiguration
    {
        public const string SectionName = "Cache";
        
        public CacheProvider Provider { get; set; } = CacheProvider.Memory;
        public string? ConnectionString { get; set; }
        public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);
        public TimeSpan SlidingExpiration { get; set; } = TimeSpan.FromMinutes(5);
    }

    public enum CacheProvider
    {
        Memory,
        Redis
    }
}