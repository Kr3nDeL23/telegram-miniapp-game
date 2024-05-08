using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Presentation.Common.Domain.Models.Response;
using Presentation.Common.Domain.Models;

namespace Presentation.Controllers;
[Authorize]
[ApiController, Route("[Controller]")]
public class CurrencyController : ControllerBase
{
    [HttpGet("[Action]")]
    public async Task<IActionResult> List()
    {
        await Task.CompletedTask;

        var response = new Response(true)
        {
            Result = new List<CurrencyModel>()
            {
                new (){
                    Name = "Tether" ,
                    Image = "/Images/currencies/tether.png" ,
                    Price = 1
                }
            }
        };
        return Ok(response);
    }
}
