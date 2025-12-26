using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebStore.Services.Interfaces;
using WebStore.ViewModels.VM;

namespace WebStore.Web.Controllers;

public class StoreApiController : BaseApiController
{
    private readonly IStoreService _storeService;

    public StoreApiController(ILogger<StoreApiController> logger, IMapper mapper, IStoreService storeService)
        : base(logger, mapper)
    {
        _storeService = storeService;
    }

    [HttpGet]
    public ActionResult Get()
    {
        // Zmiana z GetAll() na GetStores() zgodnie z Twoim interfejsem
        return Ok(_storeService.GetStores()); 
    }

    [HttpGet("{id:int}")]
    public ActionResult Get(int id)
    {
        try 
        {
            // Zmiana z Get(id) na GetStore(lambda)
            // Używamy wyrażenia lambda, bo tak wymaga Twój interfejs
            var result = _storeService.GetStore(s => s.Id == id);
            return Ok(result);
        }
        catch (Exception)
        {
            return NotFound("Store not found");
        }
    }

    [HttpPost]
    public IActionResult Post([FromBody] AddOrUpdateStoreVm modelVm)
    {
        return PostOrPutHelper(modelVm);
    }

    [HttpPut]
    public IActionResult Put([FromBody] AddOrUpdateStoreVm modelVm)
    {
        return PostOrPutHelper(modelVm);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            // Zmiana na DeleteStore(id)
            var result = _storeService.DeleteStore(id);
            if (!result) return NotFound();
            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return StatusCode(500, "Error occured");
        }
    }

    private IActionResult PostOrPutHelper(AddOrUpdateStoreVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            // Zmiana na AddOrUpdateStore
            var result = _storeService.AddOrUpdateStore(modelVm);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return StatusCode(500, "Error occured");
        }
    }
}