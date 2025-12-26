using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStore.Services.Interfaces;
using WebStore.ViewModels.VM;

namespace WebStore.Web.Controllers;

[Authorize] // Faktury to dane wrażliwe, wymagamy logowania
public class InvoiceApiController : BaseApiController
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceApiController(ILogger<InvoiceApiController> logger, IMapper mapper, IInvoiceService invoiceService)
        : base(logger, mapper)
    {
        _invoiceService = invoiceService;
    }

    // GET: api/InvoiceApi
    [HttpGet]
    public ActionResult Get()
    {
        // Pobiera wszystkie faktury
        return Ok(_invoiceService.GetInvoices());
    }

    // GET: api/InvoiceApi/5
    [HttpGet("{id:int}")]
    public ActionResult Get(int id)
    {
        try
        {
            // Pobranie jednej faktury po ID
            var result = _invoiceService.GetInvoice(i => i.Id == id);
            
            if (result == null) return NotFound("Invoice not found");
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return StatusCode(500, "Internal server error");
        }
    }

    // POST: api/InvoiceApi
    [HttpPost]
    public IActionResult Post([FromBody] AddOrUpdateInvoiceVm modelVm)
    {
        return PostOrPutHelper(modelVm);
    }

    // PUT: api/InvoiceApi
    [HttpPut]
    public IActionResult Put([FromBody] AddOrUpdateInvoiceVm modelVm)
    {
        return PostOrPutHelper(modelVm);
    }

    // DELETE: api/InvoiceApi/5
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var result = _invoiceService.DeleteInvoice(id);
            if (!result) return NotFound();
            
            return Ok("Invoice deleted successfully");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return StatusCode(500, "Error occured while deleting");
        }
    }

    // Helper do unikania powtórzeń kodu
    private IActionResult PostOrPutHelper(AddOrUpdateInvoiceVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = _invoiceService.AddOrUpdateInvoice(modelVm);
            return Ok(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return StatusCode(500, "Error occured while saving invoice");
        }
    }
}