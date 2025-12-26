using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.Services.Interfaces;
using WebStore.ViewModels.VM;

namespace WebStore.Web.Controllers;

[Authorize] // To ważne: tylko zalogowany użytkownik powinien widzieć zamówienia
public class OrderApiController : BaseApiController
{
    private readonly IOrderService _orderService;

    public OrderApiController(ILogger<OrderApiController> logger, IMapper mapper, IOrderService orderService)
        : base(logger, mapper)
    {
        _orderService = orderService;
    }

    // GET: api/OrderApi
    [HttpGet]
    public ActionResult Get()
    {
        // Pobiera wszystkie zamówienia
        // W prawdziwym sklepie filtrowalibyśmy tu po ID zalogowanego użytkownika
        return Ok(_orderService.GetOrders());
    }

    // GET: api/OrderApi/5
    [HttpGet("{id:int}")]
    public ActionResult Get(int id)
    {
        try
        {
            // Szukamy zamówienia po ID używając wyrażenia lambda (tak jak w Store)
            var result = _orderService.GetOrder(o => o.Id == id);
            
            if (result == null) return NotFound("Order not found");
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    // POST: api/OrderApi
    [HttpPost]
    public IActionResult Post([FromBody] AddOrUpdateOrderVm modelVm)
    {
        return PostOrPutHelper(modelVm);
    }

    // PUT: api/OrderApi
    [HttpPut]
    public IActionResult Put([FromBody] AddOrUpdateOrderVm modelVm)
    {
        return PostOrPutHelper(modelVm);
    }

    // DELETE: api/OrderApi/5
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var result = _orderService.DeleteOrder(id);
            if (!result) return NotFound();
            
            return Ok("Order deleted successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return StatusCode(500, "Error occured while deleting");
        }
    }

    // Metoda pomocnicza, żeby nie powtarzać kodu w Post i Put
    private IActionResult PostOrPutHelper(AddOrUpdateOrderVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Dodanie lub aktualizacja zamówienia
            var result = _orderService.AddOrUpdateOrder(modelVm);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return StatusCode(500, "Error occured while saving order");
        }
    }
}