using API2.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API2.Controllers
{
  // External API
  [Route("api/[controller]")] // api/products
  [ApiController]
  public class ProductsController : ControllerBase
  {


    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {

      // Timeout Testi için kullandık
      // Thread.Sleep(3500); // Main Thread kitledik


      // Retry testi için API2 down olmalıdır. Sonra dotnet run komutu ile API2 terminalden ayağa kaldırdığımızda aslında isteğin retry girip devam ettiğini görebiliriz.

      var plist = new List<ProductDto>();
      plist.Add(new ProductDto("P-1", 10, 12));
      plist.Add(new ProductDto("P-2", 10, 13));


      // Circuit Braker Pattern simüle edelim. Custom Exception fırlatalım.

      int a = 5;
      int b = 0;

      //int divededByZero = a / b;

      throw new Exception("Hata");

      return Ok(plist);
    }

  }
}
