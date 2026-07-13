using DZ_MP_SYSTEM_MGMT_SERVICE.API.Models.Entities;

namespace DZ_MP_SYSTEM_MGMT_SERVICE.API.Repositories;

public interface IPingRepository
{
    Task<PingEntity> GetAsync(CancellationToken cancellationToken = default);
}
