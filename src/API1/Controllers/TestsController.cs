using API1.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API1.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TestsController : ControllerBase
  {

    private readonly HttpClient api2;


    public TestsController(IHttpClientFactory httpClientFactory)
    {
      api2 = httpClientFactory.CreateClient("api2");
    }

    [HttpGet]
    public async Task<IActionResult> GetRequestAsync()
    {
      // /api/products endpoint istek at.
      //var response = await api2.GetStringAsync("/api/products");

      //var response = await api2.GetAsync("api/products");

      //if(response.IsSuccessStatusCode)
      //{
      //  var plist = response.Content.ReadFromJsonAsync<List<ProductDto>>();

      //  return Ok(plist);
      //}

      var response = await api2.GetFromJsonAsync<List<ProductDto>>("/api/products");

      return Ok(response);
    } 



  }
}
