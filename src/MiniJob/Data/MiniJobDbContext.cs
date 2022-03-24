using Microsoft.EntityFrameworkCore;
using MiniJob.Entities.Jobs;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.IdentityServer.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace MiniJob.Data;

public class MiniJobDbContext : AbpDbContext<MiniJobDbContext>
{
    public DbSet<AppInfo> AppInfos { get; set; }
    public DbSet<JobInfo> JobInfos { get; set; }
    public DbSet<JobInstance> JobInstances { get; set; }
    public DbSet<ProcessorInfo> ProcessorInfos { get; set; }
    public DbSet<WorkerInfo> WorkerInfos { get; set; }

    public MiniJobDbContext(DbContextOptions<MiniJobDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureIdentityServer();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own entities here */

        builder.ConfigureMiniJob();
    }
}
