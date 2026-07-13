using DZ_MP.CORE.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DZ_MP_BIZ_SERVICE.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PingController : ControllerBase
{
    /// <summary>Sample health-style ping for Biz service</summary>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status200OK)]
    public ActionResult<BaseResponse<object>> Get()
        => Ok(BaseResponse<object>.Ok(new { message = "pong", service = "biz" }));
}
