using Orleans.Runtime;

public interface IUrlShortenerGrain : IGrainWithStringKey
{
    Task SetUrl(string fullUrl);

    Task<string> GetUrl();
}

public sealed class UrlShortenerGrain : Grain, IUrlShortenerGrain
{
    public IPersistentState<UrlDetails> State { get; private set; }

    public UrlShortenerGrain
    (
        [PersistentState("url", "urls")] IPersistentState<UrlDetails> state
    ) => State = state;

    public Task<string> GetUrl() => Task.FromResult(State.State.FullUrl ?? string.Empty);

    public async Task SetUrl(string fullUrl)
    {
        State.State = new()
        {
            ShortenedUrl = this.GetPrimaryKeyString(),
            FullUrl = fullUrl
        };

        await State.WriteStateAsync();
    }
}
public record UrlDetails
{
    [Id(0)]
    public string FullUrl { get; set; }

    [Id(1)]
    public string ShortenedUrl { get; set; }
}
