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
        this.logger.LogWarning($"İstek tekrar deneniyor {outcome.Result}");
      });


      return response;

    }

  }

}
