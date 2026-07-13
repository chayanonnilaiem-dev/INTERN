namespace DZ_MP_SYSTEM_MGMT_SERVICE.API.Models.DTO.Response;

public class PingResponse
{
    public string Message { get; set; } = string.Empty;
    public DateTime UtcNow { get; set; }
}
