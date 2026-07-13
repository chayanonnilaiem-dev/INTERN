using Microsoft.EntityFrameworkCore;

namespace DZ_MP_BIZ_SERVICE.API.Data;

/// <summary>
/// DbContext ว่าง — พร้อมใส่ DbSet เมื่อมี entity จริง
/// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
