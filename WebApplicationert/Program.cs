using Azure.Identity;
//using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container..//
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
    app.UseSwagger();
    app.UseSwaggerUI();

var summary = Environment.GetEnvironmentVariable("WEATHER_SUMMARY");
var temparature = Random.Shared.Next(-20, 40);
//var secretClient = new SecretClient(new Uri("https://aci-keyvault1.vault.azure.net"), new DefaultAzureCredential());
//var secret = await secretClient.GetSecretAsync("test-secret1");
var secretValue = Environment.GetEnvironmentVariable("SECRET_VALUE");

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 1).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            temparature,
            summary,
            secretValue
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/loaded", () =>
{   
    //simulate load for aks task 8
    long sum = 0;
    for (int i = 0; i < 100_000_000; ++i)
    {
        sum += Random.Shared.Next(1000);
    }

    return sum;
})
.WithName("Loaded")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary, string? Secret=null)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string DeployedFrom => "Github";
}
