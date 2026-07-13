using DZ_MP_SYSTEM_MGMT_SERVICE.API.Models.DTO.Response;

namespace DZ_MP_SYSTEM_MGMT_SERVICE.API.Services;

public interface IPingService
{
    Task<PingResponse> GetAsync(CancellationToken cancellationToken = default);
}
