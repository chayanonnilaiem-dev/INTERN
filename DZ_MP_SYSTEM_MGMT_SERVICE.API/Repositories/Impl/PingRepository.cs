using DZ_MP_SYSTEM_MGMT_SERVICE.API.Models.Entities;
using DZ_MP_SYSTEM_MGMT_SERVICE.API.Repositories;

namespace DZ_MP_SYSTEM_MGMT_SERVICE.API.Repositories.Impl;

/// <summary>
/// Sample repository — ยังไม่มีตารางจริง คืนค่า in-memory อย่างเดียว
/// pattern: Repository return Entity เสมอ
/// </summary>
public class PingRepository : IPingRepository
{
    public Task<PingEntity> GetAsync(CancellationToken cancellationToken = default)
    {
        var entity = new PingEntity
        {
            Message = "pong",
            UtcNow = DateTime.UtcNow
        };
        return Task.FromResult(entity);
    }
}
