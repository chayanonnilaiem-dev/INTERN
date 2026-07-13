using Microsoft.EntityFrameworkCore;

namespace DZ_MP_SYSTEM_MGMT_SERVICE.API.Data;

/// <summary>
/// DbContext ว่าง — พร้อมใส่ DbSet เมื่อมี entity จริง
/// Schema แนะนำ: system_management (ตาม yota)
/// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // modelBuilder.HasDefaultSchema("system_management");
    }
}
