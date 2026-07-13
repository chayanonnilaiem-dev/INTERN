using DZ_MP_SYSTEM_MGMT_SERVICE.API.Models.DTO.Response;
using DZ_MP_SYSTEM_MGMT_SERVICE.API.Repositories;
using DZ_MP_SYSTEM_MGMT_SERVICE.API.Services;

namespace DZ_MP_SYSTEM_MGMT_SERVICE.API.Services.Impl;

public class PingService(IPingRepository pingRepository) : IPingService
{
    public async Task<PingResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        // Repository returns Entity — Service maps to Response DTO
        var entity = await pingRepository.GetAsync(cancellationToken);
        return new PingResponse
        {
            Message = entity.Message,
            UtcNow = entity.UtcNow
        };
    }
}
