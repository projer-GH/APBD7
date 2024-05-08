using Microsoft.AspNetCore.Mvc;
using WebApplication1.Properties.Services;

namespace WebApplication1.Properties.Controllers;


[ApiController]
[Route("api/warehouse_product")]
public class WarehouseController:ControllerBase
{


    [HttpPost]
    public async Task<IActionResult?> Add(int IdProduct, int IdWarehouse, int Amount, DateTime CreatedAt)
    {
        WarehouseService service = new WarehouseService();

        var a = service.insert(IdProduct, IdWarehouse, Amount, CreatedAt);
        
        return Ok(a);
    }
    
    
}