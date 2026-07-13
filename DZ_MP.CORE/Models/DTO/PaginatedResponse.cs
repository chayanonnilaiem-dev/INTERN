namespace DZ_MP.CORE.Models.DTO;

/// <summary>
/// คลาสสำหรับการตอบกลับข้อมูลแบบแบ่งหน้า (Pagination)
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="items">ข้อมูลที่ถูกแบ่งหน้าแล้วในรูปแบบของรายการ (List)</param>
/// <param name="totalItems">จำนวนข้อมูลทั้งหมดที่มีอยู่ (ไม่ใช่จำนวนข้อมูลในหน้าปัจจุบัน)</param>
/// <param name="pageIndex">หมายเลขหน้าปัจจุบัน</param>
/// <param name="pageSize">จำนวนข้อมูลที่แสดงในแต่ละหน้า</param>
// แบบที่ 1: สำหรับของเดิม (ไม่มี Summary)
public class PaginatedResponse<T>(List<T> items, int totalItems, int pageIndex, int pageSize)
    : PaginatedResponse<T, object?>(items, totalItems, pageIndex, pageSize, null);

/// <summary>
/// คลาสสำหรับการตอบกลับข้อมูลแบบแบ่งหน้า (Pagination)
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TSummary">ประเภทของข้อมูลสรุป</typeparam>
/// <param name="items">ข้อมูลที่ถูกแบ่งหน้าแล้วในรูปแบบของรายการ (List)</param>
/// <param name="totalItems">จำนวนข้อมูลทั้งหมดที่มีอยู่ (ไม่ใช่จำนวนข้อมูลในหน้าปัจจุบัน)</param>
/// <param name="pageIndex">หมายเลขหน้าปัจจุบัน</param>
/// <param name="pageSize">จำนวนข้อมูลที่แสดงในแต่ละหน้า</param>
/// <param name="summary">อาจจะเป็นการสรุปข้อมูลบางอย่างเพื่อไม่ให้ปนไปกับ item list</param>
// แบบที่ 2: สำหรับของใหม่ (มี Summary)
public class PaginatedResponse<T, TSummary>(List<T> items, int totalItems, int pageIndex, int pageSize, TSummary? summary = default)
{
    public List<T> Items { get; set; } = items;
    public int PageIndex { get; set; } = pageIndex;
    public int PageSize { get; set; } = pageSize;
    public int TotalPages { get; set; } = (int)Math.Ceiling(totalItems / (double)pageSize);
    public int TotalItems { get; set; } = totalItems;
    public TSummary? Summary { get; set; } = summary;
}
