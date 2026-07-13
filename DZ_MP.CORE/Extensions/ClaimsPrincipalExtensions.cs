using System.Security.Claims;

namespace DZ_MP.CORE.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// ดึงรหัสผู้ใช้งาน (UserId) จาก JWT Token ปัจจุบันที่กำลังใช้งานอยู่ (ถ้ามี)
    /// นิยมนำไปใช้ประกอบการ Insert/Update สำหรับฟิลด์ CreatedBy, UpdatedBy
    /// </summary>
    /// <returns>รหัส UserId แบบตัวเลข หรือ 0 หากไม่พบ/ไม่ได้ Login</returns>
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        if (principal == null) return 0;

        var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

        if (idClaim != null && int.TryParse(idClaim.Value, out int userId))
        {
            return userId;
        }

        return 0;
    }

    /// <summary>
    /// ดึงชื่อที่ใช้สำหรับแสดงผล (DisplayName หรือ ชื่อ-นามสกุล)
    /// </summary>
    public static string? GetDisplayName(this ClaimsPrincipal principal)
    {
        return principal?.FindFirst("display_name")?.Value;
    }

    /// <summary>
    /// ดึงประเภทผู้ใช้งาน (UserType) เช่น internal, external
    /// </summary>
    public static string? GetUserType(this ClaimsPrincipal principal)
    {
        return principal?.FindFirst("user_type")?.Value;
    }

    /// <summary>
    /// ดึง RoleId จาก JWT Token — ใช้ประกอบการเช็คสิทธิ์
    /// </summary>
    /// <returns>RoleId หรือ null ถ้าไม่พบ</returns>
    public static int? GetRoleId(this ClaimsPrincipal principal)
    {
        var value = principal?.FindFirst("role_id")?.Value;
        return int.TryParse(value, out var roleId) ? roleId : null;
    }

    /// <summary>
    /// ดึง RoleIds ทั้งหมดจาก JWT Token (รองรับ multi-role)
    /// </summary>
    /// <returns>List ของ RoleId หรือ empty list ถ้าไม่พบ</returns>
    public static List<int> GetRoleIds(this ClaimsPrincipal principal)
    {
        if (principal == null) return new List<int>();

        var value = principal.FindFirst("role_ids")?.Value;
        if (string.IsNullOrEmpty(value)) return new List<int>();

        return value.Split(',')
            .Select(id => int.TryParse(id.Trim(), out var roleId) ? roleId : (int?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToList();
    }

    /// <summary>
    /// ดึงชื่อ Role จาก JWT Token
    /// </summary>
    public static string? GetRoleName(this ClaimsPrincipal principal)
    {
        return principal?.FindFirst("role_name")?.Value;
    }

    /// <summary>
    /// ดึงชื่อ Role ทั้งหมดจาก JWT Token (รองรับ multi-role)
    /// </summary>
    /// <returns>List ของชื่อ Role หรือ empty list ถ้าไม่พบ</returns>
    public static List<string> GetRoleNames(this ClaimsPrincipal principal)
    {
        if (principal == null) return new List<string>();

        var value = principal.FindFirst("role_names")?.Value;
        if (string.IsNullOrEmpty(value)) return new List<string>();

        return value.Split(',')
            .Select(name => name.Trim())
            .Where(name => !string.IsNullOrEmpty(name))
            .ToList();
    }
}
