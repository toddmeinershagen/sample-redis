using Microsoft.Extensions.Caching.Hybrid;

public interface ICompanyServicesProvider
{
    Task<IEnumerable<string?>> GetServicesForCompanyAsync(int companyId, CancellationToken token);
}

public class CompanyServicesProvider : ICompanyServicesProvider
{
    private static readonly IDictionary<int, IEnumerable<string>> Services = new Dictionary<int, IEnumerable<string>>{
        { 1, new [] {"service-1", "service-3"} },
        { 2, new [] { "service-2" } }
    };

    private readonly HybridCache _cache;

    public CompanyServicesProvider(HybridCache cache)
    {
        this._cache = cache;
    }

    public async Task<IEnumerable<string?>> GetServicesForCompanyAsync(int companyId, CancellationToken token)
    {
        var cacheKey = $"{nameof(GetServicesForCompanyAsync)}({companyId})";
        var options = new HybridCacheEntryOptions{ Expiration = TimeSpan.FromSeconds(30)}; 

        return await _cache.GetOrCreateAsync<IEnumerable<string?>>(
            cacheKey,
            async cancel =>
            {
                Services.TryGetValue(companyId, out var result);
                return await ValueTask.FromResult(result);
            },
            options: options,
            cancellationToken: token
        );
    }
}