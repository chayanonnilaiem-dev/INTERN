namespace DZ_MP.CORE.Authorization;

public interface IPermissionService
{
    /// <summary>
    /// ตรวจสอบสิทธิ์สำหรับ Single Role
    /// </summary>
    Task RequireAsync(int roleId, int menuId, PermissionAction action);

    /// <summary>
    /// ตรวจสอบสิทธิ์สำหรับ Multi-Role (ถ้ามี Role ใดใน list ที่มีสิทธิ์ = ผ่าน)
    /// </summary>
    Task RequireAsync(List<int> roleIds, int menuId, PermissionAction action);
}
