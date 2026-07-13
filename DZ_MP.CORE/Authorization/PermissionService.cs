using System.Net;
using DZ_MP.CORE.Commons;
using DZ_MP.CORE.Exceptions;
using Microsoft.Extensions.Caching.Memory;

namespace DZ_MP.CORE.Authorization;

public class PermissionService(IPermissionRepository repository, IMemoryCache cache) : IPermissionService
{
    /// <summary>
    /// ตรวจสอบสิทธิ์สำหรับ Single Role
    /// </summary>
    public async Task RequireAsync(int roleId, int menuId, PermissionAction action)
    {
        var cacheKey = $"perm:{roleId}:{menuId}:{action}";

        if (!cache.TryGetValue(cacheKey, out bool allowed))
        {
            allowed = await repository.HasPermissionAsync(roleId, menuId, action);
            cache.Set(cacheKey, allowed, TimeSpan.FromMinutes(5));
        }

        if (!allowed)
            throw new BusinessException(HttpStatusCode.Forbidden, ErrorCode.FORBIDDEN, "คุณไม่มีสิทธิ์ดำเนินการนี้");
    }

    /// <summary>
    /// ตรวจสอบสิทธิ์สำหรับ Multi-Role (ถ้ามี Role ใดใน list ที่มีสิทธิ์ = ผ่าน)
    /// </summary>
    public async Task RequireAsync(List<int> roleIds, int menuId, PermissionAction action)
    {
        if (roleIds == null || roleIds.Count == 0)
            throw new BusinessException(HttpStatusCode.Forbidden, ErrorCode.FORBIDDEN, "ไม่พบข้อมูล Role");

        foreach (var roleId in roleIds)
        {
            var cacheKey = $"perm:{roleId}:{menuId}:{action}";

            if (!cache.TryGetValue(cacheKey, out bool allowed))
            {
                allowed = await repository.HasPermissionAsync(roleId, menuId, action);
                cache.Set(cacheKey, allowed, TimeSpan.FromMinutes(5));
            }

            if (allowed) return;
        }

        throw new BusinessException(HttpStatusCode.Forbidden, ErrorCode.FORBIDDEN, "คุณไม่มีสิทธิ์ดำเนินการนี้");
    }
}
