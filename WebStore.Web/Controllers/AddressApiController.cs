using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.Services.Interfaces;
using WebStore.ViewModels.VM;

namespace WebStore.Web.Controllers;

[Authorize] // Adresy są prywatne, wymagamy logowania
public class AddressApiController : BaseApiController
{
    private readonly IAddressService _addressService;

    public AddressApiController(ILogger<AddressApiController> logger, IMapper mapper, IAddressService addressService)
        : base(logger, mapper)
    {
        _addressService = addressService;
    }

    // GET: api/AddressApi
    [HttpGet]
    public ActionResult Get()
    {
        // Pobiera listę adresów (np. wszystkie adresy zalogowanego użytkownika)
        return Ok(_addressService.GetAddresses());
    }

    // GET: api/AddressApi/5
    [HttpGet("{id:int}")]
    public ActionResult Get(int id)
    {
        try
        {
            // Pobranie konkretnego adresu po ID
            // Używamy wyrażenia lambda zgodnie z Twoim wzorcem serwisów
            var result = _addressService.GetAddress(a => a.Id == id);
            
            if (result == null) return NotFound("Address not found");
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    // POST: api/AddressApi
    [HttpPost]
    public IActionResult Post([FromBody] AddOrUpdateAddressVm modelVm)
    {
        return PostOrPutHelper(modelVm);
    }

    // PUT: api/AddressApi
    [HttpPut]
    public IActionResult Put([FromBody] AddOrUpdateAddressVm modelVm)
    {
        return PostOrPutHelper(modelVm);
    }

    // DELETE: api/AddressApi/5
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var result = _addressService.DeleteAddress(id);
            if (!result) return NotFound();
            
            return Ok("Address deleted successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return StatusCode(500, "Error occured while deleting address");
        }
    }

    // Metoda pomocnicza
    private IActionResult PostOrPutHelper(AddOrUpdateAddressVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = _addressService.AddOrUpdateAddress(modelVm);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return StatusCode(500, "Error occured while saving address");
        }
    }
}