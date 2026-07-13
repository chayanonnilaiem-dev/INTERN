using DZ_MP.CORE.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DZ_MP_LOG_SERVICE.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PingController : ControllerBase
{
    /// <summary>Sample health-style ping for Log service</summary>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    public ActionResult<BaseResponse<object>> Get()
        => Ok(BaseResponse<object>.Ok(new { message = "pong", service = "log" }));
}
