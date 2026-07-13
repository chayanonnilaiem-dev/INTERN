namespace DZ_MP.CORE.Authorization;

public interface IPermissionRepository
{
    Task<bool> HasPermissionAsync(int roleId, int menuId, PermissionAction action);
}
