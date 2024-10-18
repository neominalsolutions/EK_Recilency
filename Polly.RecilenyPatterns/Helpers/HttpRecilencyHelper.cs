using Microsoft.Extensions.Logging;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polly.RecilenyPatterns.Helpers
{
  // 3 farklı senaryo circuit braker, timeout, retry
  public class HttpRecilencyHelper
  {
    private readonly ILogger<HttpRecilencyHelper> logger;


    public HttpRecilencyHelper(ILogger<HttpRecilencyHelper> logger)
    {
      this.logger = logger;
    }

    public IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy(int retryCount, TimeSpan sleepDuration)
    {

      var response = HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(retryCount, x =>
      {

        return sleepDuration; // 2 saniyede bir isteği toplam 3 kez dene. 6 saniye ulaşamaz ise isteği deniyecek.


      }, onRetryAsync: async (outcome, timespan, retryCount, context) =>
      {
        this.logger.LogWarning($" {retryCount}. istek atıldı");
      });


      // retryCount:  o anki deneme sayısını verir.


      return response;

    }

    // Timeout Policy
    // istek uzun sürdüğü takdirde araya girip bu isteği loglamak için bir yapı kurduk.
    public IAsyncPolicy<HttpResponseMessage> CreateTimeoutPolicy(TimeSpan timeout)
    {
      return Policy.TimeoutAsync<HttpResponseMessage>(timeout, Timeout.TimeoutStrategy.Pessimistic, (context, timespan, task) =>
      {
        logger.LogInformation($"İstek timeout düştü");

        return Task.CompletedTask;

      });
    }


    // Circuit Braker Pattern

    /// <summary>
    /// 
    /// </summary>
    /// <param name="errorCount">Kaç kez hataya girdiğinde hizmet kesintiye uğrasın. </param>
    /// <param name="timeOfBreak">Kaç saniye boyunca hata durumunda ilgili istek kesintiye uğrasın diye 
    /// belirtiğimiz değer. </param>
    /// <returns></returns>
    public IAsyncPolicy<HttpResponseMessage> CreateCircuitBrakerPolicy(int errorCount,TimeSpan timeOfBreak)
    {
      return HttpPolicyExtensions
        .HandleTransientHttpError() // 5xx hata kodları ve 408 hata kodu için 
        .Or<Exception>() // kendi exceptionlarımzada ayrıca girsin diye yaptık. Normalde HttpRequestException için çalışır.
        .CircuitBreakerAsync(errorCount, timeOfBreak,
        onBreak: (exception,duration) =>
        {
          // logger.LogInformation($"Hizmet kesintisi kısmı");

          logger.LogInformation("onBreak");
        }, 
        onReset:  () =>
        {
          logger.LogInformation("onReset");
          //logger.LogInformation("Hizmete devam et.Burada timeOfBreak kadar beklendi sonra hizmet kesintisi bitti. Circuit Braker açık konuma geçti. Taki bir daha hata olana kadar açık kalacak.");
     
        });
    }


  }

}
