using Microsoft.Extensions.Logging;
using Polly.RecilenyPatterns.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// API1 API2 recilency patternler �zerinde Request yapacak.
using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
    .SetMinimumLevel(LogLevel.Trace)
    .AddConsole());

ILogger<HttpRecilencyHelper> logger = loggerFactory.CreateLogger<HttpRecilencyHelper>();

var recilency = new HttpRecilencyHelper(logger);

builder.Services
  .AddHttpClient("api2", config =>
{
  config.BaseAddress = new Uri("http://localhost:5004"); // D�� servisler ile �al���rken haberle�me y�netmimiz bu olmal�. Mesaj Kuyruk sistemleri ile haberle�me sadece Internal olarak mant�kl�.
  // Genel olarak https://localhost:5005 UR� ise APIGatewey URI. www.a.com.

  // Mesaj Kuyruk sistemi �zerinden RequestClient ile haberle�menin avantajlar�
  // 1- Protocol veya Port bilgisinden izole oluyoruz.
  // 2- Dinamik olarak IP port de�i�imi gibi durumlardan izoleyiz.
  // 3- Hata y�netimi veya Recilency gibi kavramlar� MassTransit gibi paketlere b�rak�yoruz

}).AddPolicyHandler(recilency.CreateRetryPolicy(retryCount: 3,sleepDuration: TimeSpan.FromMinutes(1)));



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
