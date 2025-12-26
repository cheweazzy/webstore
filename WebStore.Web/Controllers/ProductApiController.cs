using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebStore.Services.Interfaces;
using WebStore.ViewModels.VM;

namespace WebStore.Web.Controllers;

public class ProductApiController : BaseApiController
{
    private readonly IProductService _productService;

    public ProductApiController(ILogger<ProductApiController> logger, IMapper mapper, IProductService productService)
        : base(logger, mapper)
    {
        _productService = productService;
    }

    [HttpGet]
    public ActionResult Get()
    {
        return Ok(_productService.GetProducts()); 
    }

    [HttpGet("{id:int}")]
    public ActionResult Get(int id)
    {
        var product = _productService.GetProduct(p => p.Id == id);
        return Ok(product);
    }

    [HttpPut]
    public IActionResult Put([FromBody] AddOrUpdateProductVm addOrUpdateProductVm)
    {
        return PostOrPutHelper(addOrUpdateProductVm);
    }

    [HttpPost]
    public IActionResult Post([FromBody] AddOrUpdateProductVm addOrUpdateProductVm)
    {
        return PostOrPutHelper(addOrUpdateProductVm);
    }

    [HttpDelete("{id:int:min(1)}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var result = _productService.DeleteProduct(p => p.Id == id);
            if (!result) return NotFound("Product not found");
            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return StatusCode(500, "Error occured");
        }
    }

    // --- DODAJ TĘ METODĘ NA DOLE ---
    private IActionResult PostOrPutHelper(AddOrUpdateProductVm addOrUpdateProductVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _productService.AddOrUpdateProduct(addOrUpdateProductVm);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return StatusCode(500, "Error occured");
        }
    }
}