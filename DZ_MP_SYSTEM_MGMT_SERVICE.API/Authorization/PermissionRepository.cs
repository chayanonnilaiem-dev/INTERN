using DZ_MP.CORE.Authorization;

namespace DZ_MP_SYSTEM_MGMT_SERVICE.API.Authorization;

/// <summary>
/// Stub PermissionRepository — คืน false ทุกครั้ง จนกว่าจะมีตาราง RoleMenuPermission
/// pattern ตาม yota: implement IPermissionRepository ใน service (query ผ่าน ApplicationDbContext)
/// </summary>
public class PermissionRepository : IPermissionRepository
{
    public Task<bool> HasPermissionAsync(int roleId, int menuId, PermissionAction action)
    {
        // TODO: query RoleMenuPermission จาก ApplicationDbContext เมื่อมี entity จริง
        return Task.FromResult(false);
    }
}
