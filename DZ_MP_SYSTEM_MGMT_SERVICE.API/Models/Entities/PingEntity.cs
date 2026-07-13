namespace DZ_MP_SYSTEM_MGMT_SERVICE.API.Models.Entities;

/// <summary>
/// Sample entity สำหรับ Ping — ยังไม่ map ตารางจริง
/// </summary>
public class PingEntity
{
    public string Message { get; set; } = string.Empty;
    public DateTime UtcNow { get; set; }
}
