using Microsoft.Extensions.Logging;
using Polly.RecilenyPatterns.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// API1 API2 recilency patternler üzerinde Request yapacak.
using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
    .SetMinimumLevel(LogLevel.Trace)
    .AddConsole());

ILogger<HttpRecilencyHelper> logger = loggerFactory.CreateLogger<HttpRecilencyHelper>();

var recilency = new HttpRecilencyHelper(logger);


builder.Services
  .AddHttpClient("api2", config =>
{
  config.BaseAddress = new Uri("http://localhost:5004"); // Dýþ servisler ile çalýþýrken haberleþme yönetmimiz bu olmalý. Mesaj Kuyruk sistemleri ile haberleþme sadece Internal olarak mantýklý.
  // Genel olarak https://localhost:5005 URÝ ise APIGatewey URI. www.a.com.

  // Mesaj Kuyruk sistemi üzerinden RequestClient ile haberleþmenin avantajlarý
  // 1- Protocol veya Port bilgisinden izole oluyoruz.
  // 2- Dinamik olarak IP port deðiþimi gibi durumlardan izoleyiz.
  // 3- Hata yönetimi veya Recilency gibi kavramlarý MassTransit gibi paketlere býrakýyoruz

})
  .AddPolicyHandler(recilency.CreateRetryPolicy(retryCount: 3, sleepDuration: TimeSpan.FromSeconds(2)))
  .AddPolicyHandler(recilency.CreateTimeoutPolicy(timeout: TimeSpan.FromSeconds(3)));
  //.AddPolicyHandler(recilency.CreateCircuitBrakerPolicy(errorCount:3,timeOfBreak: TimeSpan.FromSeconds(10)))
  //;

// Not: 3 kez ara arkaya http request iþleminde hata meydana gelirse 3 saniye api hizmetine ait endpoint tetiklenemesin. API request overload olmasýný engeller.



var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
