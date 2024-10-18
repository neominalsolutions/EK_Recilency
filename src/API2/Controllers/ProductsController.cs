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

      var plist = new List<ProductDto>();
      plist.Add(new ProductDto("P-1", 10, 12));
      plist.Add(new ProductDto("P-2", 10, 13));

      return Ok(plist);
    }

  }
}
