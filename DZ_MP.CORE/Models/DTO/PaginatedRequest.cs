namespace DZ_MP.CORE.Models.DTO;

/// <summary>
/// Base request สำหรับ pagination
/// </summary>
public class PaginatedRequest
{
    /// <summary>
    /// หน้าที่ต้องการ (default: 1)
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// จำนวนรายการต่อหน้า (default: 10)
    /// </summary>
    public int PageSize { get; set; } = 10;
}
