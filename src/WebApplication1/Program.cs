[assembly: GenerateCodeForDeclaringAssembly(typeof(UrlShortenerGrain))]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
    siloBuilder.AddMemoryGrainStorage("urls");
    siloBuilder.UseDashboard(x => x.HostSelf = true);
});



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Map("/dashboard", x => x.UseOrleansDashboard());

app.MapGet("/shorten", async (IGrainFactory grains, HttpRequest request, string url) =>
{
    var shortened = Guid.NewGuid().GetHashCode().ToString("X");

    var shortenerGrain = grains.GetGrain<IUrlShortenerGrain>(shortened);
    await shortenerGrain.SetUrl(url);

    return Results.Ok(new
    {
        Path = $"/go/{shortened}"
    });
}).WithOpenApi();

app.Run();
