using DZ_MP.CORE.Models.DTO;
using DZ_MP_SYSTEM_MGMT_SERVICE.API.Models.DTO.Response;
using DZ_MP_SYSTEM_MGMT_SERVICE.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace DZ_MP_SYSTEM_MGMT_SERVICE.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PingController(IPingService pingService) : ControllerBase
{
    /// <summary>
    /// Sample endpoint ใช้พิสูจน์ Controller → Service → Repository layer
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<PingResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<PingResponse>>> Get(CancellationToken cancellationToken)
    {
        var data = await pingService.GetAsync(cancellationToken);
        return Ok(BaseResponse<PingResponse>.Ok(data));
    }
}
